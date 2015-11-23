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
using MLA.Xna.Gui;
using MLA.Xna.Gui.Controls;
using Alliance.Pieces;
using MLA.Utilities.Algorithms;
using Alliance.Entities;
using MLA.Utilities.Algorithms.Data;
using MLA.Utilities.Helpers;

namespace Alliance
{
  public class GridComponent : DrawableGameComponent
  {
    private const int CellWidth = 15;
    private const int CellHeight = 15;
    private const int HalfThroughWay = 6;
    private const int OrthogonalCost = 10;

    private Cell[,] Cells;
    private Point[] Orthogonals = new Point[]
      {
        new Point(-1, 0),
        new Point(1, 0),
        new Point(0, -1),
        new Point(0, 1)
      };

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

    private List<Cell> mSelectedGroup;
    private List<Piece> mPieces;
    private List<Entity> mEntities;
    private Piece mSelectedPiece = null;

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
      mSelectedGroup = new List<Cell>(4);
      mPieces = new List<Piece>(100);
      mEntities = new List<Entity>(100);

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
      InitializeProperties();
      InitializeGrid();
      InitializePieceSelection();
      InitializeEntites();
    }

    private void InitializeEntites()
    {
      int count = RandomHelper.Next(10, 100);
      for (int i = 0; i < count; ++i)
      {
        Tank tank = new Tank(HorzStartCell, HorzGoalCell);
        mEntities.Add(tank);
      }
    }

    private void InitializePieceSelection()
    {
      const int ListDelta = 10;
      lstPieces = new ListBox();
      lstPieces.X = (int)(X + Width + ListDelta);
      lstPieces.Y = (int)(Y + MiddleOffset.Height);
      lstPieces.Width = (GraphicsDevice.Viewport.Width - (lstPieces.X + ListDelta));
      lstPieces.Height = lstPieces.Width;
      lstPieces.HideSelection = false;

      List<Piece> pieces = Utils.RetrieveAllSublcassesOf<Piece>();
      foreach (Piece piece in pieces)
      {
        lstPieces.Items.Add(new ListBox.ListBoxItem(piece));
      }

      mGui.Controls.Add(lstPieces);
    }

    private void InitializeGrid()
    {
      int midCol = (NumCols - 1) / 2;
      int midRow = (NumRows - 1) / 2;

      int midColLeft = midCol - HalfThroughWay;
      int midColRight = midCol + HalfThroughWay;

      int midRowUp = midRow - HalfThroughWay;
      int midRowDown = midRow + HalfThroughWay;

      Cells = new Cell[NumCols, NumRows];
      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          bool isOuter = (c == 0 || r == 0 || c == (NumCols - 1) || r == (NumRows - 1));
          bool isThroughway = isOuter && (Utils.InRange(c, midColLeft, midColRight) || Utils.InRange(r, midRowUp, midRowDown));

          Cell cell = new Cell(c, r, isOuter, isThroughway);
          cell.Bounds = new BoxF(c * CellWidth, r * CellHeight, CellWidth, CellHeight);
          Cells[c, r] = cell;
        }
      }

      HorzGoalCell = Cells[NumCols - 1, midRow];
      VertGoalCell = Cells[midCol, NumRows - 1];

      HorzStartCell = Cells[0, midRow];
      VertStartCell = Cells[midCol, 0];

      SolveGrid();
    }

    private void InitializeProperties()
    {
      base.Initialize();

      Width = (int)((double)GraphicsDevice.Viewport.Height * .85);
      Height = Width;

      NumCols = (Width / CellWidth) - 1;
      NumRows = (Height / CellHeight) - 1;

      MiddleOffset = new SizeF(
        (Width / 2f) - ((NumCols * CellWidth) / 2f),
        (Height / 2f) - ((NumRows * CellHeight) / 2f));

      mPosition = new Vector2(0f, (GraphicsDevice.Viewport.Height / 2f) - (Height / 2f));
    }

    public override void Update(GameTime gameTime)
    {
      InputProvider input = (InputProvider)Game.Services.GetService(typeof(InputProvider));

      PlaceSelectedPiece(input);
      UpdateEntities(gameTime);
      UpdatePieces(gameTime, input);

      base.Update(gameTime);
    }

    private void UpdatePieces(GameTime gameTime, InputProvider input)
    {
      bool pieceSelected = mSelectedPiece != null && mSelectedPiece.Selected;

      if (input.SellRequested && pieceSelected)
      {
        int idx = mPieces.IndexOf(mSelectedPiece);
        if (idx > -1)
        {
          mPieces[idx].Sell();
        }
      }
      else if (input.UpgradeRequested && pieceSelected)
      {
        int idx = mPieces.IndexOf(mSelectedPiece);
        if (idx > -1)
        {
          mPieces[idx].Upgrade();
        }
      }

      lock (mPieces)
      {
        int count = mPieces.Count;
        for (int i = mPieces.Count - 1; i > -1; --i)
        {
          if (mPieces[i].State == PieceState.Sold)
          {
            mPieces.RemoveAt(i);
          }
        }

        if (count != mPieces.Count)
          SolveGrid();
      }

      foreach (Piece piece in mPieces)
      {
        piece.Update(gameTime);
      }
    }

    private void UpdateEntities(GameTime gameTime)
    {
      for (int i = mEntities.Count - 1; i > -1; --i)
      {
        Entity entity = mEntities[i];
        entity.Update(gameTime);

        bool remove = false;
        switch (entity.State)
        {
          case EntityState.MadeIt:
            PlayAlright();
            remove = true;
            break;
          case EntityState.Dead:
            remove = true;
            break;
        }

        if (remove)
          mEntities.RemoveAt(i);
      }
    }

    private void PlayAlright()
    {
      AllianceGame.Sounds.PlayCue("alright");
    }

    private void PlaceSelectedPiece(InputProvider input)
    {
      if (lstPieces.SelectedIndex > -1)
      {
        SelectPieceGroup(input.CursorPosition);
        if (mSelectedGroup.Count > 0)
        {
          bool isBlocked = false;
          Piece piece = null;

          for (int i = 0; !isBlocked && i < mSelectedGroup.Count; ++i)
          {
            isBlocked |= (mSelectedGroup[i].Type == CellType.Blocked);
            piece = (piece == null ? mSelectedGroup[i].Piece : piece);
          }

          if (input.AddClick)
          {
            if (!isBlocked)
            {
              OnAddClick();
            }
            else
            {
              if (piece.Equals(mSelectedPiece))
              {
                piece.Selected = !piece.Selected;
              }
              else
              {
                if (mSelectedPiece != null)
                  mSelectedPiece.Selected = false;

                mSelectedPiece = piece;
                mSelectedPiece.Selected = true;
              }
            }
          }
        }
      }
    }

    private void RemoveSelectedPiece()
    {
      foreach (Cell cell in mSelectedGroup)
      {
        mPieces.Remove(cell.Piece);
        cell.RemovePiece();
      }
      SolveGrid();
    }

    private void OnAddClick()
    {
      lock (mPieces)
      {
        ListBox.ListBoxItem item = lstPieces.Items[lstPieces.SelectedIndex];
        Piece piece = item.Value as Piece;
        if (piece != null)
        {
          mSelectedPiece = null;
          Piece placed = piece.Place(mSelectedGroup.ToArray());
          mPieces.Add(placed);

          // solve the grid with A*
          AStarNode[,] nodes = new AStarNode[NumCols, NumRows];
          for (int c = 0; c < NumCols; ++c)
          {
            for (int r = 0; r < NumRows; ++r)
            {
              nodes[c, r] = Cells[c, r].ToAStarNode();
            }
          }

          // if we can't get through, then remove the piece and alert the user
          AStarNode start = nodes[HorzStartCell.Column, HorzStartCell.Row];
          AStarNode end = nodes[HorzGoalCell.Column, HorzGoalCell.Row];
          bool pathExists = AStar.PathExists(nodes, AStarMovement.Orthogonal, start, end);

          // if we CAN get through
          if (pathExists)
          {
            // solve the grid with Dijkstra
            SolveGrid();

            // query all the entities and ask them if they can get through.
            for (int i = 0; pathExists && i < mEntities.Count; ++i)
            {
              pathExists &= mEntities[i].CanPlacePiece(placed);
            }
          }

          // if they can't get through, then remove the piece and alert the user
          if (!pathExists)
          {
            RemoveSelectedPiece();
          }
        }
      }
    }

    private void SelectPieceGroup(Vector2 cursorPos)
    {
      mSelectedGroup.Clear();
      Vector2 offset = (Vector2)MiddleOffset + mPosition;

      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          Cell node = Cells[c, r];
          if (node.IsOuter) continue;

          BoxF bounds = new BoxF(node.X + offset.X, node.Y + offset.Y, node.Width, node.Height);
          if (bounds.Contains(cursorPos))
          {
            // select the piece cell
            SelectPieceCell(c, r);

            // we're done searching
            c = NumCols;
            r = NumRows;
          }
        }
      }
    }

    private void SelectPieceCell(int c, int r)
    {
      int over = c + 1;
      int down = r + 1;

      if (over < NumCols && down < NumRows)
      {
        // get the collection of cells needed
        Cell uLeft = Cells[c, r];
        Cell uRight = Cells[over, r];
        Cell lRight = Cells[over, down];
        Cell lLeft = Cells[c, down];

        // determine if the cell is an outer cell
        bool isOuter = (uLeft.IsOuter || uRight.IsOuter || lRight.IsOuter || lLeft.IsOuter);

        // as long as the cell is valid
        if (!isOuter)
        {
          // add the cells to the selected group of cells
          mSelectedGroup.Add(uLeft);
          mSelectedGroup.Add(uRight);
          mSelectedGroup.Add(lRight);
          mSelectedGroup.Add(lLeft);
        }
      }
    }

    public override void Draw(GameTime gameTime)
    {
      mSpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

      DrawGrid();
      DrawPieces();
      DrawEntities();

      mSpriteBatch.End();
      base.Draw(gameTime);
    }

    private void DrawEntities()
    {
      lock (mEntities)
      {
        Vector2 offset = (Vector2)MiddleOffset + mPosition;
        foreach (Entity entity in mEntities)
        {
          entity.Draw(mSpriteBatch, offset);
        }
      }
    }

    private void DrawPieces()
    {
      lock (mPieces)
      {
        Vector2 offset = (Vector2)MiddleOffset + mPosition;
        foreach (Piece piece in mPieces)
        {
          piece.Selected = (piece.Equals(mSelectedPiece) && mSelectedPiece.Selected);
          piece.Draw(mSpriteBatch, offset);
        }
      }
    }

    private void DrawGrid()
    {
      Vector2 offset = (Vector2)MiddleOffset + mPosition;
      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          Cell node = Cells[c, r];
          BoxF bounds = new BoxF(node.X + offset.X, node.Y + offset.Y, node.Width, node.Height);

          if (node.IsOuter && !node.IsThroughway)
          {
            Shapes.FillRectangle(mSpriteBatch, bounds, Color.White);
          }
          else if (mSelectedGroup.Contains(node))
          {
            Shapes.FillRectangle(mSpriteBatch, bounds, Color.Pink);
          }
        }
      }
    }

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

      Q.Add(HorzGoalCell);
      HorzGoalCell.ResetDijkstra();
      HorzGoalCell.Distance = 0;

      while (Q.Count > 0)
      {
        Q.Sort();
        Cell u = Q[0];
        Q.RemoveAt(0);

        foreach (Cell v in GetAdjacent(u))
        {
          if (IsIllegal(v)) continue;

          int alt = u.Distance + OrthogonalCost;
          if (alt < v.Distance)
          {
            v.Distance = alt;
            v.Parent = u;
          }
        }
      }
    }

    private bool IsIllegal(Cell node)
    {
      return node.Type != CellType.Empty || (!node.IsThroughway && node.IsOuter);
    }

    private IEnumerable<Cell> GetAdjacent(Cell node)
    {
      foreach (Point pt in Orthogonals)
      {
        int c = node.Column + pt.X;
        int r = node.Row + pt.Y;

        if ((-1 < c && c < NumCols) && (-1 < r && r < NumRows))
        {
          Cell adjacent = Cells[c, r];
          yield return adjacent;
        }
      }
    }
  }
}
