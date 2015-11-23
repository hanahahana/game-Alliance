using System;
using System.Collections.Generic;
using System.Text;

using MLA.Xna.Gui;
using MLA.Xna.Gui.Controls;
using MLA.Utilities.Algorithms;
using MLA.Utilities.Algorithms.Data;
using MLA.Utilities.Helpers;
using MLA.Utilities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Pieces;
using Alliance.Entities;
using Alliance.Projectiles;
using Alliance.Parameters;
using Alliance.Helpers;

namespace Alliance.Components
{
  public partial class GridComponent : DrawableGameComponent
  {
    private const int CellWidth = 20;
    private const int CellHeight = 20;
    private const int HalfThroughWay = 6;
    private const int OrthogonalCost = 12;

    private Cell[,] Cells;
    private int NumRows;
    private int NumCols;
    private int Width;
    private int Height;
    private Cell HorzGoalCell;
    private Cell VertGoalCell;
    private Cell HorzStartCell;
    private Cell VertStartCell;
    private SizeF MiddleOffset;
    private SpriteBatch mSpriteBatch;
    private Vector2 mPosition;
    private SelectionPiece mSelectionPiece;
    private Piece mSelectedPiece;
    private Entity mSelectedEntity;

    private List<Piece> mPieces;
    private List<Entity> mEntities;
    private List<Projectile> mProjectiles;

    private GuiManager mGui;
    private ListBox lstPieces;
    private TextBox txtDescription;
    private Caption cptDescription;
    private SpriteFont captionFont;
    private ShapeBatch shapeBatch;
    private BoxF currentArea = BoxF.Empty;

    public GridComponent(Game game)
      : base(game)
    {
      game.IsMouseVisible = true;

      mGui = new GuiManager(game);
      game.Components.Add(mGui);
    }

    protected override void LoadContent()
    {
      mSpriteBatch = new SpriteBatch(GraphicsDevice);
      shapeBatch = new ShapeBatch(GraphicsDevice);
      captionFont = AllianceGame.Fonts["ComicSans"];
    }

    public override void Initialize()
    {
      base.Initialize();
      Player.InitializePlayer(100000, 500);
      InitializeVariables();
      InitializeProperties();
      InitializeGrid();
      InitializeListBox();
      InitializeDescriptionBox();
    }

    public override void Update(GameTime gameTime)
    {
      // immediately add some invaders
      AddInvaders();

      // get the input provider
      InputProvider input = (InputProvider)Game.Services.GetService(typeof(InputProvider));

      // construct new updata parameters for the functions
      UpdateParams uparams = new UpdateParams(gameTime, input, (Vector2)MiddleOffset + mPosition);

      // update the list
      UpdateSelectionList(uparams);

      // update the selection the user has made
      UpdateSelectionPiece(uparams);

      // process any requests from the user for a piece
      ProcessInput(uparams);

      // update the description text that is displayed
      UpdateDescriptionText(uparams);

      // update the radius around the selected piece
      UpdateSelectedPieceRadius(uparams);

      // update all pieces
      UpdatePieces(uparams);

      // update all invaliders
      UpdateInvaders(uparams);

      // update the piece targets
      UpdatePieceTargets(uparams);

      // update all projectiles
      UpdateProjectiles(uparams);

      base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      DrawParams dparams = new DrawParams(gameTime, (Vector2)MiddleOffset + mPosition, mSpriteBatch);
      mSpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

      DrawGrid(dparams);
      DrawPieces(dparams);
      DrawInvaders(dparams);
      DrawProjectiles(dparams);

      mSpriteBatch.End();

      DrawPieceRadius();
      base.Draw(gameTime);
    }

    #region Solve Grid
    private void SolveGrid()
    {
      SolveGrid(HorzGoalCell, DijkstraType.Horizontal);
      SolveGrid(VertGoalCell, DijkstraType.Vertical);
    }

    private void SolveGrid(Cell goalCell, DijkstraType dijkstraType)
    {
      List<Cell> Q = new List<Cell>(Cells.Length);
      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          Cell v = Cells[c, r];
          if (!IsIllegal(v))
          {
            v[dijkstraType].Reset();
            Q.Add(v);
          }
        }
      }

      goalCell[dijkstraType].Reset();
      goalCell[dijkstraType].Distance = 0;

      while (Q.Count > 0)
      {
        // extra the minimum value
        Cell u = ExtractMin(dijkstraType, ref Q);

        // relax each of the neighbors
        foreach (Cell v in u.AdjacentCells)
        {
          if (v == null || IsIllegal(v)) continue;
          Relax(dijkstraType, v, u);
        }
      }
    }

    private Cell ExtractMin(DijkstraType dijkstraType, ref List<Cell> Q)
    {
      int min = int.MaxValue;
      int idx = 0;

      for (int i = 0; i < Q.Count; ++i)
      {
        if (Q[i][dijkstraType].Distance < min)
        {
          min = Q[i][dijkstraType].Distance;
          idx = i;
        }
      }

      Cell retval = Q[idx];
      Q.RemoveAt(idx);

      return retval;
    }

    private void Relax(DijkstraType dijkstraType, Cell v, Cell u)
    {
      int alt = u[dijkstraType].Distance + OrthogonalCost;
      if (alt < v[dijkstraType].Distance)
      {
        v[dijkstraType].Distance = alt;
        v[dijkstraType].Parent = u;
      }
    }

    private bool IsIllegal(Cell node)
    {
      return node.Type != CellType.Empty || (!node.IsThroughway && node.IsOuter);
    }
    #endregion
  }
}