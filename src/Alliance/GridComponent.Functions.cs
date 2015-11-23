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
using Alliance.Projectiles;

using MLA.Xna.Gui;
using MLA.Xna.Gui.Controls;
using MLA.Utilities.Algorithms;
using MLA.Utilities.Algorithms.Data;
using MLA.Utilities.Helpers;
using MLA.Utilities;

namespace Alliance
{
  public partial class GridComponent
  {
    #region Properties

    protected ContentManager Content { get { return Game.Content; } }

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

    #endregion

    #region Initialization Functions

    private void InitializeVariables()
    {
      mPieces = new List<Piece>(100);
      mEntities = new List<Entity>(100);
      mProjectiles = new List<Projectile>(100);
      mSelectionPiece = new SelectionPiece();
    }

    private void InitializeListBox()
    {
      const int ListDelta = 10;
      lstPieces = new ListBox();
      lstPieces.X = (int)(X + Width + ListDelta);
      lstPieces.Y = (int)(Y + MiddleOffset.Height);
      lstPieces.Width = (GraphicsDevice.Viewport.Width - (lstPieces.X + ListDelta));
      lstPieces.Height = (int)(lstPieces.Width * .70f);
      lstPieces.HideSelection = false;

      List<Piece> pieces = Utils.RetrieveAllSublcassesOf<Piece>();
      foreach (Piece piece in pieces)
      {
        lstPieces.Items.Add(new ListBoxItem(piece));
      }

      mGui.Controls.Add(lstPieces);
    }

    private void InitializeDescriptionBox()
    {
      cptDescription = new Caption();
      cptDescription.X = lstPieces.X;
      cptDescription.Y = (lstPieces.Y + lstPieces.Height) + 4;
      cptDescription.Width = lstPieces.Width;
      cptDescription.Height = (int)Math.Round(captionFont.MeasureString("Z").Y + 5);
      cptDescription.BackColor = new Color(226, 201, 160);
      cptDescription.BorderColor = Color.Blue;
      cptDescription.BorderThickness = 1;
      cptDescription.Font = captionFont;
      cptDescription.CenterCaption = true;

      txtDescription = new TextBox();
      txtDescription.X = lstPieces.X;
      txtDescription.Y = (cptDescription.Y + cptDescription.Height - 1);
      txtDescription.Width = lstPieces.Width;
      txtDescription.Height = (int)(Height - (Y + lstPieces.Height + cptDescription.Height - 13));

      txtDescription.BackColor = Color.White;
      txtDescription.BorderColor = Color.Blue;
      txtDescription.BorderThickness = 1;

      mGui.Controls.Add(cptDescription);
      mGui.Controls.Add(txtDescription);
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

      Point[] Orthogonals = new Point[]
      {
        new Point(-1, 0),
        new Point(1, 0),
        new Point(0, -1),
        new Point(0, 1)
      };

      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          Cell node = Cells[c, r];
          foreach (Point pt in Orthogonals)
          {
            int aC = node.Column + pt.X;
            int aR = node.Row + pt.Y;

            if ((-1 < aC && aC < NumCols) && (-1 < aR && aR < NumRows))
            {
              Cell adjacent = Cells[aC, aR];
              node.Add(adjacent);
            }
          }
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
      Width = (int)((double)GraphicsDevice.Viewport.Height * .85);
      Height = Width;

      NumCols = (Width / CellWidth) - 1;
      NumRows = (Height / CellHeight) - 1;

      MiddleOffset = new SizeF(
        (Width / 2f) - ((NumCols * CellWidth) / 2f),
        (Height / 2f) - ((NumRows * CellHeight) / 2f));

      mPosition = new Vector2(0f, (GraphicsDevice.Viewport.Height / 2f) - (Height / 2f));
    }

    #endregion

    #region Helper Functions

    private void AddInvaders()
    {
      if (RandomHelper.NextRareBool())
      {
        int count = RandomHelper.Next(3, 7);
        for (int i = 0; i < count; ++i)
        {
          Tank tank = new Tank(HorzStartCell, HorzGoalCell);
          mEntities.Add(tank);
        }
      }
    }

    private void OnSelectClick(UpdateParams uparams)
    {
      // always clear the current selection
      ClearSelection();

      // if the they click with a valid piece, then place it
      if (mSelectionPiece.IsValid)
      {
        // build the piece in the location
        Piece piece = mSelectionPiece.Build();
        mPieces.Add(piece);

        // if we can't get through
        if (!AssertCanGetThrough(piece))
        {
          // display a blocking message
          Messages.AddMessage("Blocking",
            mPosition + new Vector2((Width / 2f), (Height / 2f)),
            600,
            true);

          // remove the piece
          int last = mPieces.Count - 1;
          mPieces.RemoveAt(last);
          piece.Clear();

          // re-solve the grid
          SolveGrid();
        }
      }
      else
      {
        // the piece is invalid. Find the piece that they clicked on.
        bool found = false;
        for (int i = 0; !found && i < mPieces.Count; ++i)
        {
          Piece piece = mPieces[i];
          BoxF box = new BoxF(piece.Position + uparams.Offset, piece.Size);
          if (box.Contains(uparams.Input.CursorPosition))
          {
            // set selection
            found = true;
            SetSelection(piece);
          }
        }
      }
    }

    private bool AssertCanGetThrough(Piece piece)
    {
      // if the piece isn't blocking, then we can get through
      if (!piece.IsBlocking) return true;

      // check the horizontal path
      AStarNode[,] aStarGrid = BuildAStarGrid();
      AStarNode start = aStarGrid[HorzStartCell.Column, HorzStartCell.Row];
      AStarNode end = aStarGrid[HorzGoalCell.Column, HorzGoalCell.Row];

      // if there is a path from the start to the end
      if (!AStar.PathExists(aStarGrid, AStarMovement.Orthogonal, start, end)) return false;

      // solve the current grid
      SolveGrid();

      // if each of the entities can get through
      bool canGetThrough = true;
      for (int i = 0; canGetThrough && i < mEntities.Count; ++i)
      {
        Entity entity = mEntities[i];
        canGetThrough &= entity.CanPlacePiece(piece);
      }

      // return if we can get through
      return canGetThrough;
    }

    private AStarNode[,] BuildAStarGrid()
    {
      AStarNode[,] grid = new AStarNode[NumCols, NumRows];
      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          grid[c, r] = Cells[c, r].ToAStarNode();
        }
      }
      return grid;
    }

    private void ClearSelection()
    {
      // if there is a selection
      if (mSelectedPiece != null)
      {
        // clear it away
        mSelectedPiece.Selected = false;
      }

      // set the selected piece to null
      mSelectedPiece = null;

      // set the selected entity to null
      mSelectedEntity = null;
    }

    private void SetSelection(Piece newPiece)
    {
      // if the new piece doesn't equal the current piece, then
      // the current piece needs to be cleared.
      if (!newPiece.Equals(mSelectedPiece))
      {
        // if there is no selection, then this will be null. Otherwise, there is a selection,
        // so first clear it away
        ClearSelection();

        // set the current selected piece to the new piece
        mSelectedPiece = newPiece;
      }

      // clear away the selected piece
      lstPieces.SelectedIndex = -1;

      // toggle the selected property of the piece
      mSelectedPiece.Selected = !mSelectedPiece.Selected;
    }

    private Cell[] GetCellsWithCenterContainedIn(BoxF cursorBox, Vector2 offset)
    {
      List<Cell> cells = new List<Cell>(100);
      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          Cell cell = Cells[c, r];
          Vector2 center = cell.Bounds.Location + offset;
          center.X += (cell.Width / 2f);
          center.Y += (cell.Height / 2f);
          if (cursorBox.Contains(center))
          {
            cells.Add(cell);
          }
        }
      }
      return cells.ToArray();
    }

    private Piece RetrieveCurrentSelectedPiece(UpdateParams uparams)
    {
      Piece piece = null;

      bool validSelection = lstPieces.SelectedIndex > -1 || mSelectedPiece != null;
      if (validSelection)
      {
        piece = mSelectedPiece;
        if (piece == null)
        {
          piece = lstPieces.Items[lstPieces.SelectedIndex].Value as Piece;
          Piece.FollowSelectionPiece(piece, mSelectionPiece);
        }
      }

      return piece;
    }

    private Entity RetrieveCurrentSelectedEntity(UpdateParams uparams)
    {
      Entity retval = mSelectedEntity;
      if (retval != null && retval.State != EntityState.Alive)
        retval = null;

      if (uparams.Input.SelectClick)
      {
        // if they click, then reset the selection
        retval = null;
        for (int i = mEntities.Count - 1; i > -1; --i)
        {
          Entity entity = mEntities[i];
          if (entity.State == EntityState.Alive)
          {
            BoxF box = entity.GetBoundingBox(uparams.Offset);
            if (box.Contains(uparams.Input.CursorPosition))
            {
              retval = entity;
              i = -1;
            }
          }
        }
      }

      mSelectedEntity = retval;
      return retval;
    }

    private void DisplayCaptionForTextProvider(ITextProvider provider)
    {
      txtDescription.Text = provider.GetText();
    }

    private void DisplayDescriptionForTextProvider(ITextProvider provider)
    {
      cptDescription.Text = provider.GetHeader();
    }

    private void OnProjectileRemoved(Projectile projectile)
    {
      if (projectile is MissileProjectile)
      {
        DebriProjectile[] debris = DebriProjectile.Create(projectile, RandomHelper.Next(12, 24));
        mProjectiles.AddRange(debris);
      }
    }

    private void CheckCollisions(Projectile projectile, UpdateParams uparams)
    {
      // get the bounding box of the projectile
      BoxF bounds = projectile.GetBoundingBox(uparams.Offset);
      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          Cell cell = Cells[c, r];
          if (cell.RegisteredEntitiesCount > 0)
          {
            // get the cell bounds and offset it by the ...offset
            BoxF cbounds = cell.Bounds;
            cbounds.Location += uparams.Offset;

            // if the cell intersects with the projectile 
            // or the cell contains the projectile
            if (cbounds.IntersectsWith(bounds) || cbounds.Contains(bounds))
            {
              // get the most recent entity and attack it!
              Entity entity = cell.GetMostRecentRegisteredEntity();
              projectile.IsAlive = false;
              entity.CurrentLife -= projectile.Attack;

              // if this entity is dead, then remove it from the cell
              if (entity.CurrentLife <= 0)
                cell.Unregister(entity);

              // we're done checking
              r = NumRows;
              c = NumCols;
            }
          }
        }
      }
    }

    #endregion

    #region Update Functions

    private void UpdatePieces(UpdateParams uparams)
    {
      bool resolve = false;
      for (int i = mPieces.Count - 1; i > -1; --i)
      {
        Piece piece = mPieces[i];
        piece.Update(uparams.GameTime);

        if (piece.State == PieceState.Sold)
        {
          // remove the piece
          mPieces.RemoveAt(i);

          // if the currently selected piece is this piece, then set it to null
          if (piece.Equals(mSelectedPiece))
            mSelectedPiece = null;

          // if we remove a piece the grid needs to be re-solved
          resolve = true;
        }

        Projectile[] projectiles = piece.PopProjectiles();
        if (piece.State == PieceState.Idle)
        {
          mProjectiles.AddRange(projectiles);
        }
      }

      if (resolve)
        SolveGrid();
    }

    private void ProcessInput(UpdateParams uparams)
    {
      // if the user selects a group of cells
      if (uparams.Input.SelectClick)
      {
        // process the selection click
        OnSelectClick(uparams);
      }

      bool selectedPieceValid = mSelectedPiece != null && mSelectedPiece.Selected;
      if (uparams.Input.SellRequested && selectedPieceValid)
      {
        mSelectedPiece.Sell();
      }
      if (uparams.Input.UpgradeRequested && selectedPieceValid)
      {
        mSelectedPiece.Upgrade();
      }
      if (uparams.Input.ClearSelections)
      {
        lstPieces.SelectedIndex = -1;
        ClearSelection();
      }
    }

    private void UpdateSelectionPiece(UpdateParams uparams)
    {
      if (lstPieces.SelectedIndex < 0)
      {
        // clear the selection
        mSelectionPiece.ClearSelection();
      }
      else
      {
        // the user has something selected in the list. Our goal is to show the user the
        // available places where their selection can be set.

        // get the piece thats associated with the selected item
        Piece piece = lstPieces.Items[lstPieces.SelectedIndex].Value as Piece;

        // figure out how much space this piece needs. This piece needs cellsNeeded by cellsNeeded
        int cellsNeeded = (int)piece.Grouping;

        // create a list to hold how many cells will be needed
        int capacity = cellsNeeded * cellsNeeded;
        List<Cell> group = new List<Cell>(capacity);

        // if we only need one cell
        if (cellsNeeded == 1)
        {
          // find the cell containing the cursor position
          Index index = Utils.GetIndexCorrespondingTo(uparams.Input.CursorPosition, CellWidth, CellHeight, uparams.Offset);
          if (Utils.IndexValid(index, NumCols, NumRows))
          {
            group.Add(Cells[index.C, index.R]);
          }
        }
        else if (cellsNeeded > 1)
        {
          // if we need more cells, then select the cells whose center are contained within the current
          // cursor position
          float width = cellsNeeded * CellWidth;
          float height = cellsNeeded * CellHeight;
          BoxF cursorBox = new BoxF(
            uparams.Input.CursorPosition.X - (width / 2),
            uparams.Input.CursorPosition.Y - (height / 2),
            width,
            height);
          Cell[] cells = GetCellsWithCenterContainedIn(cursorBox, uparams.Offset);
          group.AddRange(cells);
        }

        // if we found the group of cells, then set the new selection
        if (group.Count == capacity)
        {
          /// Set the current selection. This will determine the size of the selection when drawing, and the
          /// color based on the group of cells.
          mSelectionPiece.SetSelection(group, piece);
        }
        else
        {
          // the mouse isn't near anything, so just clear it
          mSelectionPiece.ClearSelection();
        }
      }
    }

    private void UpdateDescriptionText(UpdateParams uparams)
    {
      Piece piece = RetrieveCurrentSelectedPiece(uparams);
      Entity entity = RetrieveCurrentSelectedEntity(uparams);

      txtDescription.Visible = false;
      cptDescription.Visible = false;

      if (piece != null || entity != null)
      {
        txtDescription.Visible = true;
        cptDescription.Visible = true;

        ITextProvider provider = (piece != null ? (ITextProvider)piece : (ITextProvider)entity);
        DisplayDescriptionForTextProvider(provider);
        DisplayCaptionForTextProvider(provider);
      }
    }

    private void UpdateInvaders(UpdateParams uparams)
    {
      for (int i = mEntities.Count - 1; i > -1; --i)
      {
        Entity entity = mEntities[i];
        entity.Update(uparams.GameTime);
        if (entity.State != EntityState.Alive)
        {
          mEntities.RemoveAt(i);
          if (entity.State == EntityState.MadeIt)
          {
            Player.InvaderGotThrough(entity);
            //AllianceGame.Sounds.PlayCue("alright");
          }
          else
          {
            Player.CollectSpoils(entity);
            //AllianceGame.Sounds.PlayCue("kaching");
          }
        }
      }
    }

    private void UpdateSelectedPieceRadius(UpdateParams uparams)
    {
      Piece piece = RetrieveCurrentSelectedPiece(uparams);
      currentArea = BoxF.Empty;

      bool validPiece = (piece != null) && (piece.State == PieceState.Idle);
      if (validPiece && !piece.Size.IsEmpty)
      {
        float dx = piece.Width * .5f;
        float dy = piece.Height * .5f;
        currentArea = new BoxF(piece.X + dx, piece.Y + dy, piece.Radius, piece.Radius);
      }
    }

    private void UpdatePieceTargets(UpdateParams uparams)
    {
      foreach (Piece piece in mPieces)
      {
        // clear out the current target
        piece.ClearTarget();
      }

      foreach (Entity invader in mEntities)
      {
        // define the center of the invader
        BoxF box = invader.Bounds;

        // find all of the pieces that this invader is contained within
        List<Piece> pieces = mPieces.FindAll(new Predicate<Piece>(delegate(Piece piece)
        {
          // get the center of the circle
          Vector2 center = new Vector2(piece.X + (piece.Width * .5f), piece.Y + (piece.Height * .5f));

          // return if the circle contains the box
          return Utils.CircleContains(center, piece.Radius, box);
        }));

        // for each piece, set their current target
        foreach (Piece piece in pieces)
        {
          piece.SetTarget(invader);
        }
      }
    }

    private void UpdateProjectiles(UpdateParams uparams)
    {
      BoxF viewport = new BoxF(X, Y, Width, Height);
      for (int i = mProjectiles.Count - 1; i > -1; --i)
      {
        Projectile projectile = mProjectiles[i];
        projectile.Update(uparams.GameTime);

        // if the projectile is still alive, check to see if it went out of bounds
        if (projectile.IsAlive)
        {
          projectile.IsAlive = viewport.Contains(projectile.Bounds);
        }

        // if the projectile is still alive, check to see if it hit anything
        if (projectile.IsAlive)
        {
          CheckCollisions(projectile, uparams);
        }

        // if the projectile is dead, then remove it from the list
        if (!projectile.IsAlive)
        {
          mProjectiles.RemoveAt(i);
          OnProjectileRemoved(projectile);
        }
      }
    }

    #endregion

    #region Drawing Functions

    private void DrawPieces(DrawParams dparams)
    {
      foreach (Piece piece in mPieces)
      {
        piece.Draw(dparams);
      }
      mSelectionPiece.Draw(dparams.SpriteBatch, dparams.Offset);
    }

    private void DrawGrid(DrawParams dparams)
    {
      Vector2 offset = dparams.Offset;
      SpriteBatch spriteBatch = dparams.SpriteBatch;

      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          Cell node = Cells[c, r];
          BoxF bounds = new BoxF(node.X + offset.X, node.Y + offset.Y, node.Width, node.Height);

          if (node.IsOuter && !node.IsThroughway)
          {
            Shapes.FillRectangle(spriteBatch, bounds, Color.White);
          }

          if ((node.Attributes & DebugAttributes.OccupiedByProjectile) != 0)
          {
            Shapes.FillRectangle(spriteBatch, bounds, Color.Red);
            node.Attributes = DebugAttributes.None;
          }
        }
      }
    }

    private void DrawInvaders(DrawParams dparams)
    {
      foreach (Entity invader in mEntities)
      {
        invader.Draw(dparams);
      }
    }

    private void DrawPieceRadius()
    {
      if (!currentArea.IsEmpty)
      {
        Vector2 offset = (Vector2)MiddleOffset + mPosition;
        Vector2 pos = currentArea.Location + offset;
        Vector2 size = new Vector2(currentArea.Width, currentArea.Height);

        shapeBatch.Begin(SaveStateMode.SaveState);
        shapeBatch.FillEllipse(pos, size, Utils.NewAlpha(Color.SlateBlue, .5f));
        shapeBatch.DrawEllipse(pos, size, Color.Gold);
        shapeBatch.End();
      }
    }

    private void DrawProjectiles(DrawParams dparams)
    {
      foreach (Projectile projectile in mProjectiles)
      {
        projectile.Draw(dparams);
      }
    }

    #endregion
  }
}
