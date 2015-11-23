using System;
using System.Collections.Generic;
using System.Text;

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

using MLA.Xna.Gui;
using MLA.Xna.Gui.Controls;
using MLA.Utilities.Algorithms;
using MLA.Utilities.Algorithms.Data;
using MLA.Utilities.Helpers;
using MLA.Utilities;

namespace Alliance
{
  public partial class GridComponent : DrawableGameComponent
  {
    private const int CellWidth = 15;
    private const int CellHeight = 15;
    private const int HalfThroughWay = 6;
    private const int OrthogonalCost = 10;

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

    private List<Piece> mPieces;
    private List<Entity> mEntities;

    private GuiManager mGui;
    private ListBox lstPieces;

    public Vector2 Position
    {
      get { return mPosition; }
      set { mPosition = value; }
    }

    public float X
    {
      get { return mPosition.X; }
      set { mPosition.X = value; }
    }

    public float Y
    {
      get { return mPosition.Y; }
      set { mPosition.Y = value; }
    }

    public GridComponent(Game game)
      : base(game)
    {
      game.IsMouseVisible = true;

      mGui = new GuiManager(game);
      game.Components.Add(mGui);
    }

    protected override void LoadContent()
    {
      base.LoadContent();
      mSpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    public override void Initialize()
    {
      InitializeVariables();
      InitializeProperties();
      InitializeGrid();
      InitializeListBox();
    }

    public override void Update(GameTime gameTime)
    {
      // get the input provider
      InputProvider input = (InputProvider)Game.Services.GetService(typeof(InputProvider));

      // get the current offset
      Vector2 offset = (Vector2)MiddleOffset + mPosition;

      // construct new updata parameters for the functions
      UpdateParams uparams = new UpdateParams(gameTime, input, offset);

      // update the selection the user has made
      UpdateSelectionPiece(uparams);

      // if the user selects a group of cells
      if (input.SelectClick)
      {
        // process the selection click
        OnSelectClick(uparams);
      }

      // process any requests from the user for a piece
      ProcessPieceRequests(uparams);

      // update all pieces
      UpdatePieces(uparams);

      base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      mSpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

      DrawGrid();
      DrawPieces();

      mSpriteBatch.End();
      base.Draw(gameTime);
    }

    #region Solve Grid
    private void SolveGrid()
    {
      List<Cell> Q = new List<Cell>(Cells.Length);
      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          Cell v = Cells[c, r];
          if (!IsIllegal(v))
          {
            v.ResetDijkstra();
            Q.Add(v);
          }
        }
      }

      HorzGoalCell.ResetDijkstra();
      HorzGoalCell.Distance = 0;

      while (Q.Count > 0)
      {
        // extra the minimum value
        Cell u = ExtractMin(ref Q);

        // relax each of the neighbors
        foreach (Cell v in u.AdjacentCells)
        {
          if (v == null || IsIllegal(v)) continue;
          Relax(v, u);
        }
      }
    }

    private Cell ExtractMin(ref List<Cell> Q)
    {
      int min = int.MaxValue;
      int idx = 0;

      for (int i = 0; i < Q.Count; ++i)
      {
        if (Q[i].Distance < min)
        {
          min = Q[i].Distance;
          idx = i;
        }
      }

      Cell retval = Q[idx];
      Q.RemoveAt(idx);

      return retval;
    }

    private void Relax(Cell v, Cell u)
    {
      int alt = u.Distance + OrthogonalCost;
      if (alt < v.Distance)
      {
        v.Distance = alt;
        v.Parent = u;
      }
    }

    private bool IsIllegal(Cell node)
    {
      return node.Type != CellType.Empty || (!node.IsThroughway && node.IsOuter);
    }
    #endregion
  }
}
