using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  public partial class AllianceManager
  {
    public const int CellWidth = 20;
    public const int CellHeight = 20;
    public const int HalfThroughWay = 5;
    public const int OrthogonalCost = 12;

    public const double SecondsToFlashCell = 1.1;
    public const double FlashesPerSecond = 8.4848484848484;
    public const int MaxFlashes = 5;

    private static readonly Color FlashStart;
    private static readonly Color FlashEnd;

    private GridCell[,] Grid;
    private AStarNode[,] AStarGrid;
    private DijkstraPathsGrid DijkstraGrid;
    private int NumRows;
    private int NumCols;
    private int Width;
    private int Height;
    private GridCell HorzGoalCell;
    private GridCell VertGoalCell;
    private GridCell HorzStartCell;
    private GridCell VertStartCell;
    private Vector2 MiddleOffset;
    private SpriteBatch mSpriteBatch;
    private Selection mSelection;
    private Piece mSelectedPiece;
    private Invader mSelectedInvader;
    private List<Piece> mPieces;
    private List<Invader> mInvaders;
    private List<Projectile> mProjectiles;
    private InputState input;
    private GuiComponent mGui;
    private ListBox lstPieces;
    private TextBox txtDescription;
    private Button btnSell;
    private Button btnUpgrade;
    private SpriteFont captionFont;
    private PrimitiveGraphics mGraphics;
    private RectangleF currentArea = RectangleF.Empty;
    private Dictionary<GridCell, double> cellsToFlash;
    private InvaderMothership invaderMothership;
    private RectangleF gridBounds;
    private List<Piece> pieceObjects;

    public GridFillMode FillMode { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public bool GameOver { get; set; }
    public bool GameWon { get; set; }

    public Vector2 Position
    {
      get { return new Vector2(X, Y); }
      set { X = value.X; Y = value.Y; }
    }

    static AllianceManager()
    {
      FlashStart = Color.Red;
      FlashEnd = Color.Pink.NewAlpha(100);
    }

    public AllianceManager(Game game)
    {
      FillMode = GridFillMode.Solid;
      input = new InputState(game);
      GameOver = false;
      GameWon = false;
    }

    public override void LoadContent()
    {
      mSpriteBatch = new SpriteBatch(GraphicsDevice);
      mGraphics = new PrimitiveGraphics(mSpriteBatch);

      mGui = new GuiComponent(Game, mSpriteBatch);
      mGui.AllControlsVisibleWhenAdded = false;
      mGui.LoadContent();

      captionFont = Program.Resources.Fonts["ComicSans"];
      Player.InitializePlayer(500000, 500000);

      InitializeVariables();
      InitializeProperties();
      InitializeGrid();
      InitializePathFinding();
      InitializeListBox();
      InitializeDescriptionBox();
      InitializeButtons();
    }

    public override void Update(GameTime gameTime)
    {
      // update the visible pieces so they get updated right away
      UpdateVisiblePieces();

      // immediately add the waves of invaders (if necessary)
      AddInvaders();

      // update the input
      input.Update(gameTime);

      // update the gui
      mGui.Update(gameTime);

      // construct new updata parameters for the functions
      UpdateParams uparams = new UpdateParams(gameTime, input, (Vector2)MiddleOffset + Position);

      // update the list
      UpdateSelectionList(uparams);

      // update the selection the user has made
      UpdateSelection(uparams);

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
    }

    public override void Draw(GameTime gameTime)
    {
      DrawParams dparams = new DrawParams(gameTime, (Vector2)MiddleOffset + Position, FillMode, mSpriteBatch, mGraphics);
      mSpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

      DrawGrid(dparams);
      DrawPieces(dparams);
      DrawInvaders(dparams);
      DrawProjectiles(dparams);
      DrawPieceRadius(dparams);

      mSpriteBatch.End();
      mGui.Draw(gameTime);
    }

    #region Functions

    #region Initialization Functions

    private void InitializeVariables()
    {
      mPieces = new List<Piece>(100);
      mInvaders = new List<Invader>(100);
      mProjectiles = new List<Projectile>(100);
      mSelection = new Selection();
      cellsToFlash = new Dictionary<GridCell, double>();
      invaderMothership = new InvaderMothership();
    }

    private void InitializeListBox()
    {
      const int ListDelta = 10;
      lstPieces = new ListBox();
      lstPieces.X = (int)(X + Width + ListDelta);
      lstPieces.Y = (int)(Y + MiddleOffset.Height);
      lstPieces.Width = (GraphicsDevice.Viewport.Width - (lstPieces.X + ListDelta));
      lstPieces.Height = (int)(lstPieces.Width * .60f);
      lstPieces.HideSelection = false;
      lstPieces.DisplayStyle = ListBoxDisplayStyle.ImageAndText;
      mGui.Controls.Add(lstPieces);

      List<Type> pieceTypes = ReflectionHelper.RetrieveAllSublcassTypes(typeof(Piece), typeof(Piece).Assembly);
      pieceObjects = ReflectionHelper.CreateInstances<Piece>(pieceTypes);
      pieceObjects.ForEach(p => p.DisplayUpgradeInfo = false);
      pieceObjects.Sort((a, b) => a.LevelVisibility.CompareTo(b.LevelVisibility));

      lstPieces.ImageList = new List<AImage>(pieceObjects.Count);
      lstPieces.Visible = true;
    }

    private void InitializeDescriptionBox()
    {
      txtDescription = new TextBox();
      txtDescription.X = lstPieces.X;
      txtDescription.Y = lstPieces.Bounds.Bottom + 5;
      txtDescription.Width = lstPieces.Width;
      txtDescription.Height = Height - (lstPieces.Bounds.Bottom + 5);

      txtDescription.BackColor = Color.White;
      txtDescription.BorderColor = Color.Blue;
      txtDescription.BorderThickness = 1;
      txtDescription.Visible = false;

      mGui.Controls.Add(txtDescription);
    }

    private void InitializeButtons()
    {
      Point center = txtDescription.Bounds.Center;
      const int BtnOffset = 6;

      SpriteFont font = AllianceGame.Fonts["BookmanOldStyle"];
      int width = Math.Min(80, (txtDescription.Width / 2) - BtnOffset);
      int height = 25;
      int y = txtDescription.Bounds.Bottom - (height + BtnOffset);

      btnSell = new Button();
      btnSell.Text = "Sell";
      btnSell.Font = font;
      btnSell.Width = width;
      btnSell.Height = height;
      btnSell.X = center.X - (width + BtnOffset);
      btnSell.Y = y;
      btnSell.BackColor = Color.Green;
      btnSell.Visible = false;
      btnSell.TextAlignment = DataAlignment.MiddleCenter;

      btnUpgrade = new Button();
      btnUpgrade.Text = "Upgrade";
      btnUpgrade.Font = font;
      btnUpgrade.Width = width;
      btnUpgrade.Height = height;
      btnUpgrade.X = center.X + BtnOffset;
      btnUpgrade.Y = y;
      btnUpgrade.BackColor = Color.Red;
      btnUpgrade.Visible = false;
      btnUpgrade.TextAlignment = DataAlignment.MiddleCenter;

      btnSell.Click += new EventHandler(btnSell_Click);
      btnUpgrade.Click += new EventHandler(btnUpgrade_Click);

      mGui.Controls.Add(btnSell);
      mGui.Controls.Add(btnUpgrade);
    }

    private void InitializeGrid()
    {
      int midCol = (NumCols - 1) / 2;
      int midRow = (NumRows - 1) / 2;

      int midColLeft = midCol - HalfThroughWay;
      int midColRight = midCol + HalfThroughWay;

      int midRowUp = midRow - HalfThroughWay;
      int midRowDown = midRow + HalfThroughWay;

      int dc = (NumCols - 1) % 2;
      int dr = (NumRows - 1) % 2;

      Grid = new GridCell[NumCols, NumRows];
      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          bool isOuter = (c == 0 || r == 0 || c == (NumCols - 1) || r == (NumRows - 1));
          bool isThroughway = isOuter && (ArithmeticHelper.InRange(c, midColLeft + dc, midColRight) || ArithmeticHelper.InRange(r, midRowUp + dr, midRowDown));

          GridCell cell = new GridCell(c, r, isOuter, isThroughway);
          cell.Bounds = new BoxF(c * CellWidth, r * CellHeight, CellWidth, CellHeight);
          Grid[c, r] = cell;
        }
      }

      HorzGoalCell = Grid[NumCols - 1, midRow];
      VertGoalCell = Grid[midCol, NumRows - 1];

      HorzStartCell = Grid[0, midRow];
      VertStartCell = Grid[midCol, 0];

      gridBounds = BoxF.FromLTRB(Grid[0, 0].X, Grid[0, 0].Y, Grid[NumCols - 1, 0].X + CellWidth, Grid[0, NumRows - 1].Y + CellHeight);
      gridBounds.Offset((Vector2)MiddleOffset + Position);
    }

    private void InitializePathFinding()
    {
      AStarGrid = new AStarNode[NumCols, NumRows];
      DijkstraGrid = new DijkstraPathsGrid();

      DijkstraGrid[DijkstraType.LeftToRight] = new DijkstraNode[NumCols, NumRows];
      DijkstraGrid[DijkstraType.TopToBottom] = new DijkstraNode[NumCols, NumRows];

      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          // get the grid cell
          GridCell cell = Grid[c, r];

          // create an A* node
          AStarGrid[c, r] = new AStarNodeAlliance(cell);

          // create the Dijkstra nodes
          DijkstraGrid[DijkstraType.LeftToRight][c, r] = new DijkstraNodeAlliance(cell, DijkstraType.LeftToRight);
          DijkstraGrid[DijkstraType.TopToBottom][c, r] = new DijkstraNodeAlliance(cell, DijkstraType.TopToBottom);

          // set the goal cells
          if (HorzGoalCell.Equals(cell))
            DijkstraGrid.SetGoal(DijkstraType.LeftToRight, c, r);
          if (VertGoalCell.Equals(cell))
            DijkstraGrid.SetGoal(DijkstraType.TopToBottom, c, r);
        }
      }

      SolveGrid();
    }

    private void InitializeProperties()
    {
      Width = (int)((double)GraphicsDevice.Viewport.Height * .85);
      Height = Width;

      NumCols = (Width / CellWidth) - 1;
      NumRows = (Height / CellHeight) - 1;

      MiddleOffset = new Vector2(
        (Width / 2f) - ((NumCols * CellWidth) / 2f),
        (Height / 2f) - ((NumRows * CellHeight) / 2f));

      X = 0;
      Y = (GraphicsDevice.Viewport.Height / 2f) - (Height / 2f);
    }

    #endregion

    #region Helper Functions

    private void SolveGrid()
    {
      DijkstraGrid.Solve(DijkstraType.LeftToRight);
      DijkstraGrid.Solve(DijkstraType.TopToBottom);
    }

    private void AddInvaders()
    {
      // update the HudComponent
      HudComponent.AllowSkipping = !invaderMothership.SentAllInvaders;

      // if we sent all the invaders, and there are no more invaders here, then we won!
      if (invaderMothership.SentAllInvaders && mInvaders.Count == 0)
      {
        // the game has been won!
        GameWon = true;
      }

      // check the timespan
      if (Player.TimeUntilInvadersArrive <= TimeSpan.Zero)
      {
        // add the wave of invaders
        GenerateNextWave();

        // after they're added, reset the time
        Player.TimeUntilInvadersArrive = TimeSpan.FromSeconds(30.0);
      }
    }

    private void GenerateNextWave()
    {
      // get the data needed for the mothership
      Tuple<GridCell, GridCell> horizontal = new Tuple<GridCell, GridCell>(HorzStartCell, HorzGoalCell);
      Tuple<GridCell, GridCell> vertical = new Tuple<GridCell, GridCell>(VertStartCell, VertGoalCell);

      // generate the next wave
      IEnumerable<Invader> invaders = invaderMothership.GenerateWave(horizontal, vertical, mPieces, Grid);
      mInvaders.AddRange(invaders);

      // trigger the design phase as over
      if (Player.State == PlayerState.Designing)
        Player.TriggerDesignPhaseOver();

      // sort the invaders so that the flying ones are drawn last
      mInvaders.Sort((a, b) => a.Flying.CompareTo(b.Flying));
    }

    private void OnSelectClick(UpdateParams uparams)
    {
      // ignore this function if the buttons have the mouse
      if (btnSell.MouseCaptured || btnUpgrade.MouseCaptured)
        return;

      // always clear the current selection
      ClearSelection();

      // if the user has no selection, then select the piece
      if (lstPieces.SelectedIndex == -1)
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
      ResetAStarGrid();
      AStarNode startHorz = AStarGrid[HorzStartCell.Column, HorzStartCell.Row];
      AStarNode endHorz = AStarGrid[HorzGoalCell.Column, HorzGoalCell.Row];

      // if there is a path from the start to the end
      if (!AStar.PathExists(AStarGrid, AStarMovement.Orthogonal, startHorz, endHorz)) return false;

      // check the vertical path
      ResetAStarGrid();
      AStarNode startVert = AStarGrid[VertStartCell.Column, VertStartCell.Row];
      AStarNode endVert = AStarGrid[VertGoalCell.Column, VertGoalCell.Row];

      // if there is a path from the start to the end
      if (!AStar.PathExists(AStarGrid, AStarMovement.Orthogonal, startVert, endVert)) return false;

      // solve the current grid
      SolveGrid();

      // if each of the entities can get through
      bool canGetThrough = true;
      for (int i = 0; canGetThrough && i < mInvaders.Count; ++i)
      {
        Invader invader = mInvaders[i];
        canGetThrough &= invader.CanPlacePiece(piece);
      }

      // return if we can get through
      return canGetThrough;
    }

    private void ResetAStarGrid()
    {
      // every node in the existing grid needs to be updated with the new data
      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          // reset the A* Node.
          AStarGrid[c, r].Reset();
        }
      }
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
      mSelectedInvader = null;
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

    private Piece RetrieveCurrentInformationPiece(UpdateParams uparams)
    {
      Piece retval = null;

      // The hovered piece information is NOT saved, so we only care about which 
      // piece is being hovered over before displaying text. Note that the hovered piece will
      // take precendence over the selected invader. If an invader is selected,
      // and we hover over a piece, then the piece will display it's information.
      // once the player hovers out of the piece, the selected invader's information
      // will come back. Also note that the hovered piece takes precedence over the currently selected
      // piece.

      // get the piece that's currently being hovered over
      retval = RetrieveCurrentHoveredPiece(uparams);
      if (retval == null)
      {
        // if it's null, then get the selected piece
        retval = RetrieveCurrentSelectedPiece(uparams);
      }

      // return whichever piece. If this is null, it means no piece is hovered or selected
      return retval;
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
          if (mSelection != null)
          {
            piece.Bounds = mSelection.Bounds;
          }
        }
      }

      return piece;
    }

    private Piece RetrieveCurrentHoveredPiece(UpdateParams uparams)
    {
      Piece piece = null;
      if (lstPieces.HoverIndex > -1)
      {
        // set the hovered piece
        piece = lstPieces.Items[lstPieces.HoverIndex].Value as Piece;
      }
      return piece;
    }

    private Invader RetrieveCurrentSelectedInvader(UpdateParams uparams)
    {
      Invader retval = mSelectedInvader;
      if (retval != null && retval.State != InvaderState.Alive)
        retval = null;

      if (uparams.Input.SelectObject)
      {
        // if they click, then reset the selection
        retval = null;
        for (int i = mInvaders.Count - 1; i > -1; --i)
        {
          Invader invader = mInvaders[i];
          if (invader.State == InvaderState.Alive)
          {
            BoxF box = invader.GetBoundingBox(uparams.Offset);
            if (box.Contains(uparams.Input.CursorPosition))
            {
              retval = invader;
              i = -1;
            }
          }
        }
      }

      mSelectedInvader = retval;
      return retval;
    }

    private void DisplayCaptionForTextProvider(ITextDisplay provider)
    {
      txtDescription.Text = provider.GetText();
    }

    private void DisplayDescriptionForTextProvider(ITextDisplay provider)
    {
      // here, we display the text
      // provider.GetHeader();
    }

    private void OnProjectileRemoved(Projectile projectile)
    {
      if (projectile is MissileProjectile)
      {
        DebriProjectile[] debris = DebriProjectile.Create(projectile, RandomHelper.Next(3, 9));
        mProjectiles.AddRange(debris);
      }
    }

    private void CheckCollisions(Projectile projectile, Polygon projectilePolygon, UpdateParams uparams)
    {
      // if the projectile parent is null, then don't worry about it
      if (projectile.Parent == null)
        return;

      // cycle through all of the entities
      for (int i = mInvaders.Count - 1; i > -1; --i)
      {
        // get the entity
        Invader invader = mInvaders[i];

        // if this invader isn't the type of invader that the projectile can hit, then keep going
        if (!projectile.CanHit(invader))
          continue;

        // get the entity polygon
        Polygon entityPolygon = invader.GetHull(uparams.Offset);

        // let the entity and polygon know they were attacked
        if (entityPolygon.IntersectsWith(projectilePolygon))
        {
          // let the projectile know it collided
          projectile.OnCollidedWithInvader(invader);

          // let the entity know it was attacked
          invader.OnAttackedByProjectile(projectile);
        }
      }
    }

    private void ProcessInput(UpdateParams uparams)
    {
      // if the user selects a group of cells
      if (uparams.Input.SelectObject)
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

    private void AddChunksHorz(Vector2 offset, Vector2 mousePt, int cellsNeeded, bool left)
    {
      // create a list to hold the chunk pieces
      List<GridCell> chunkPieces = new List<GridCell>();

      // get the starting chunk
      GridCellChunk startingChunk = mSelection.StartingChunk;

      // preprocess some values
      float startY = startingChunk.Y;
      float startX = startingChunk.X + (left ? -CellWidth : startingChunk.Width);
      float addY = CellHeight;
      float addX = CellWidth * (left ? -1 : 1);
      float endY = startingChunk.Bounds.Bottom;
      float endX = mousePt.X + (cellsNeeded == 1 ? 0 : addX);
      int comparison = (left ? 1 : -1);

      // start at the bottom of the chunk and move down until we hit the cursor y
      for (float x = startX; x.CompareTo(endX).Equals(comparison); x += addX)
      {
        for (float y = startY; y < endY; y += addY)
        {
          UpdateChunkPieces(ref chunkPieces, cellsNeeded, offset, x, y);
        }
      }
    }

    private void AddChunksVert(Vector2 offset, Vector2 mousePt, int cellsNeeded, bool above)
    {
      // create a list to hold the chunk pieces
      List<GridCell> chunkPieces = new List<GridCell>();

      // get the starting chunk
      GridCellChunk startingChunk = mSelection.StartingChunk;

      // preprocess some values
      float startY = startingChunk.Y + (above ? -CellHeight : startingChunk.Height);
      float startX = startingChunk.X;
      float addY = CellHeight * (above ? -1 : 1);
      float addX = CellWidth;
      float endY = mousePt.Y + (cellsNeeded == 1 ? 0 : addY);
      float endX = startingChunk.Bounds.Right;
      int comparison = (above ? 1 : -1);

      // move to the end y
      for (float y = startY; y.CompareTo(endY).Equals(comparison); y += addY)
      {
        for (float x = startX; x < endX; x += addX)
        {
          UpdateChunkPieces(ref chunkPieces, cellsNeeded, offset, x, y);
        }
      }
    }

    public Dictionary<string, object> GetSaveData()
    {
      Dictionary<string, object> saveData = new Dictionary<string, object>();

      saveData["Towers"] = mPieces;
      saveData["Invaders"] = mInvaders;
      saveData["Grid"] = Grid;
      saveData["HorzGoalCell"] = HorzGoalCell;
      saveData["VertGoalCell"] = VertGoalCell;
      saveData["HorzStartCell"] = HorzStartCell;
      saveData["VertStartCell"] = VertStartCell;
      saveData["NumRows"] = NumRows;
      saveData["NumCols"] = NumCols;
      saveData["InvaderWave"] = invaderMothership;

      return saveData;
    }

    public void LoadSaveData(Dictionary<string, object> saveData)
    {
      mPieces = (List<Piece>)saveData["Towers"];
      mInvaders = (List<Invader>)saveData["Invaders"];
      Grid = (GridCell[,])saveData["Grid"];
      HorzGoalCell = (GridCell)saveData["HorzGoalCell"];
      VertGoalCell = (GridCell)saveData["VertGoalCell"];
      HorzStartCell = (GridCell)saveData["HorzStartCell"];
      VertStartCell = (GridCell)saveData["VertStartCell"];
      NumRows = (int)saveData["NumRows"];
      NumCols = (int)saveData["NumCols"];
      invaderMothership = (InvaderMothership)saveData["InvaderWave"];

      InitializePathFinding();
    }

    #endregion

    #region Event Handlers

    private void btnUpgrade_Click(object sender, EventArgs e)
    {
      if (mSelectedPiece != null)
      {
        mSelectedPiece.Upgrade();
      }
    }

    private void btnSell_Click(object sender, EventArgs e)
    {
      if (mSelectedPiece != null)
      {
        mSelectedPiece.Sell();
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
          {
            mSelectedPiece = null;
          }

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

    private void UpdateVisiblePieces()
    {
      // from here, add items that don't exist
      int i = lstPieces.ImageList.Count;
      foreach (Piece piece in pieceObjects)
      {
        // determine if the item already exists
        bool contains = lstPieces.Items.Find(listboxitem =>
        {
          return object.ReferenceEquals(listboxitem.Value, piece);
        }) != null;

        // if the player can see this item, and we can't find a list box item that
        // has the piece set as it's value, then let's add it!
        if ((Player.Level >= piece.LevelVisibility) && !contains)
        {
          // add a new item
          ListBoxItem item = new ListBoxItem(piece);
          item.ImageIndex = (i++);
          item.ImageColor = Color.Gray;
          lstPieces.Items.Add(item);

          // add the piece image
          lstPieces.ImageList.Add(piece.GetDisplayImage());
        }
      }
    }

    private void UpdateSelectionList(UpdateParams uparams)
    {
      // show the list and see if we can select a piece
      lstPieces.Visible = true;
      foreach (ListBoxItem item in lstPieces.Items)
      {
        Piece piece = item.Value as Piece;
        if (piece != null)
        {
          // this item can only be selected if the player has enough cash for it
          item.CanSelect = Player.EnoughCashFor(piece);
        }
      }
    }

    private void UpdateSelection(UpdateParams uparams)
    {
      // the user has something selected in the list. Our goal is to show the user the
      // available places where their selection can be set.

      // update the current piece selection
      UpdatePiecePlacement(uparams);

      // update the flashing cells
      UpdateFlashingCells(uparams);
    }

    private void UpdateFlashingCells(UpdateParams uparams)
    {
      List<GridCell> keys = cellsToFlash.Keys.ToList();
      foreach (GridCell cell in keys)
      {
        double amount = cellsToFlash[cell];
        amount -= uparams.GameTime.ElapsedGameTime.TotalSeconds;
        cellsToFlash[cell] = amount;
        if (amount <= 0)
        {
          // remove the cell so it doesn't get drawn OR updated
          cellsToFlash.Remove(cell);
        }
      }
    }

    private void UpdatePiecePlacement(UpdateParams uparams)
    {
      // set the chunk to null
      mSelection.Chunk = null;

      // don't place any towers if no towers are selected
      if (lstPieces.SelectedIndex == -1)
      {
        // end any edits that are in place
        mSelection.EndPlaceEdits();
        return;
      }

      // get the mouse point
      Vector2 mousePt = uparams.Input.CursorPosition - uparams.Offset;

      // get the current chunk
      Piece piece = lstPieces.Items[lstPieces.SelectedIndex].Value as Piece;
      int cellsNeeded = (int)piece.Grouping;
      float width = CellWidth * cellsNeeded;
      float height = CellHeight * cellsNeeded;

      // compute the data needed
      Vector2 offset = Vector2.Zero;
      BoxF cursorBox = AllianceUtilities.CreateBoxAroundPoint(mousePt, offset, width, height);
      GridCell[] cells = AllianceUtilities.GetCellsWithCenterContainedIn(Grid, cursorBox, offset);

      if (cells.Length > 0)
      {
        BoxF bounds = AllianceUtilities.ComputeBounds(cells, CellWidth, CellHeight);
        mSelection.Chunk = new GridCellChunk
        {
          Bounds = bounds,
          Cells = cells,
          Valid = (cells.Length == (cellsNeeded * cellsNeeded)) && Array.TrueForAll<GridCell>(cells, GridCell.CellIsAvailable),
        };
      }

      // set the bounds of the selection to be the bounds of the chunk
      mSelection.Bounds = BoxF.Empty;
      if (mSelection.Chunk != null)
      {
        mSelection.Bounds = mSelection.Chunk.Bounds;
      }

      // if the mouse is down and we haven't added any chunks, then make this the starting chunk
      if (uparams.Input.SelectPressed)
      {
        // if we don't have a starting chunk, then start it
        if (mSelection.StartingChunk == null)
        {
          mSelection.StartingChunk = mSelection.Chunk;
        }

        // clear the bounds
        mSelection.Bounds = BoxF.Empty;

        // get the starting chunk for easy access
        GridCellChunk startingChunk = mSelection.StartingChunk;

        // if the starting chunk is null, or invalid, then don't do this
        if (startingChunk != null && startingChunk.Valid)
        {
          // basically, we need to determine how many chunks to add to the chunks currently being
          // edited. We can do this by first determining where the cursor box is. If it is below us
          // (meaning the top of the cursor box is greater than the bottom of the starting chunk
          // box), then we just need to count and see how many cells away the [0,0] cell from the cursor
          // box is away from the starting chunk box. So, "int countNeeded = cellsAwayFromStartingBox - 
          // (cellsAwayFromStartingBox % cellsNeeded". This will tell us how many cells we should collect
          // in each direction (horz and vert). Then, we divide that number by cellsNeeded to get the number
          // of chunks. Then, we simply assemble the chunks and add them to a collection. The same logic is
          // done if the cursor box is horizontally away.

          // clear away the chunks being edited
          mSelection.ClearEdits();

          // figure out if the cursor is in the vert plane, or the horz plane
          float dy = mousePt.Y - startingChunk.Bounds.Center.Y;
          float dx = mousePt.X - startingChunk.Bounds.Center.X;
          float aDy = Math.Abs(dy);
          float aDx = Math.Abs(dx);

          if (aDy > aDx)
          {
            // the difference in the y direction is greater. If dy > 0, this means that the cursor is BELOW the starting chunk. 
            // If dy < 0, this means that the cursor is ABOVE the starting chunk. If dy == 0, then we don't care
            if (dy != 0)
            {
              AddChunksVert(offset, mousePt, cellsNeeded, dy < 0);
            }
          }
          else
          {
            // the difference in the x direction is greater. If dx > 0, this means that the cursor is RIGHT of the starting chunk.
            // If dx < 0, this means that the cursor is LEFT of the starting chunk. Again, dx == 0, we don't care
            if (dx != 0)
            {
              AddChunksHorz(offset, mousePt, cellsNeeded, dx < 0);
            }
          }

          // make sure that the chunk we're drawing is the starting chunk
          mSelection.Chunk = startingChunk;
        }
      }
      else
      {
        // if we have a starting chunk
        if (mSelection.StartingChunk != null)
        {
          // add all the towers
          foreach (GridCellChunk editedChunk in mSelection.Edits)
          {
            // see if we have enough funds
            bool enoughFunds = Player.EnoughCashFor(piece);

            // create a default invalid message
            string invalidMessage = enoughFunds ? "Cell(s) not available" : "Not enough funds";

            // update the validity of the chunk
            editedChunk.Valid &= enoughFunds;

            // if the chunk is valid, then attempt to place it.
            if (editedChunk.Valid)
            {
              // build the piece in the location
              Piece tower = piece.BuildFromChunk(gridBounds, editedChunk);
              mPieces.Add(tower);

              // if we can't get through
              if (!AssertCanGetThrough(tower))
              {
                // remove the piece
                int last = mPieces.Count - 1;
                tower.SellInstant();
                mPieces.RemoveAt(last);

                // re-solve the grid
                SolveGrid();

                // the piece is no longer valid
                editedChunk.Valid = false;

                // blocking
                invalidMessage = "Blocking Invaders";
              }
            }

            if (!editedChunk.Valid)
            {
              // add a quick message
              MessageComponent.AddMessage(invalidMessage,
                  Position + new Vector2((Width / 2f), (Height / 2f)),
                  800,
                  true);

              // a chunk can be invalid if all the cells don't have towers.
              // Find any cells that have towers, and flash them red
              foreach (GridCell cell in editedChunk.Cells)
              {
                if (cell.IsWalkable)
                {
                  // we use a dictionary so we don't add the same cell twice. We also
                  // reset the time that the cell is flashing.
                  cellsToFlash[cell] = SecondsToFlashCell;
                }
              }
            }
          }
        }

        mSelection.EndPlaceEdits();
      }
    }

    private void UpdateChunkPieces(ref List<GridCell> chunkPieces, int cellsNeeded, Vector2 offset, float x, float y)
    {
      int r = (int)((y - offset.Y) / CellHeight);
      int c = (int)((x - offset.X) / CellWidth);

      if ((-1 < r && r < NumRows) && (-1 < c && c < NumCols))
      {
        // add this cell to the current chunk
        chunkPieces.Add(Grid[c, r]);
      }

      // if we have the correct number of chunks
      if (chunkPieces.Count == (cellsNeeded * cellsNeeded))
      {
        // compute the bounds of the cell pieces
        BoxF bounds = AllianceUtilities.ComputeBounds(chunkPieces, CellWidth, CellHeight);

        // for now, we only care if the chunk is available (in terms of it being valid)
        GridCellChunk item = new GridCellChunk
        {
          Bounds = bounds,
          Cells = chunkPieces.ToArray(),
          Valid = chunkPieces.TrueForAll(GridCell.CellIsAvailable),
        };
        mSelection.AddChunk(item);
        chunkPieces.Clear();
      }
    }

    private void UpdateDescriptionText(UpdateParams uparams)
    {
      Piece piece = RetrieveCurrentInformationPiece(uparams);
      Invader invader = RetrieveCurrentSelectedInvader(uparams);

      txtDescription.Visible = false;
      btnSell.Visible = (piece != null & mSelectedPiece != null && mSelectedPiece.Equals(piece));
      btnUpgrade.Visible = btnSell.Visible;
      btnUpgrade.Enabled = btnSell.Visible && piece.CanUpgrade;

      if (piece != null || invader != null)
      {
        txtDescription.Visible = true;
        ITextDisplay provider = (piece != null ? (ITextDisplay)piece : (ITextDisplay)invader);
        DisplayDescriptionForTextProvider(provider);
        DisplayCaptionForTextProvider(provider);
      }
    }

    private void UpdateInvaders(UpdateParams uparams)
    {
      for (int i = mInvaders.Count - 1; i > -1; --i)
      {
        Invader entity = mInvaders[i];
        entity.Update(uparams.GameTime);
        if (entity.State != InvaderState.Alive)
        {
          mInvaders.RemoveAt(i);
          if (entity.State == InvaderState.MadeIt)
          {
            Player.InvaderGotThrough(entity);
            if (!Player.IsAlive)
            {
              GameOver = true;
              i = -1;
            }
          }
          else
          {
            Player.CollectSpoils(entity);
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
      Vector2 offset = (Vector2)MiddleOffset + Position;
      mPieces.ForEach(piece => piece.ChooseTarget(mInvaders, offset));
    }

    private void UpdateProjectiles(UpdateParams uparams)
    {
      float dx = CellWidth * 2f;
      float dy = CellHeight * 2f;

      BoxF viewport = new BoxF(X - dx, Y - dy, Width + dx, Height + dy);
      for (int i = mProjectiles.Count - 1; i > -1; --i)
      {
        // get the projectile
        Projectile projectile = mProjectiles[i];

        // update the projectile data
        projectile.Update(uparams.GameTime);

        // get the polygon data
        Polygon polygon = projectile.GetHull(uparams.Offset);

        // if the projectile is still alive, check to see if it went out of bounds
        if (projectile.IsAlive)
        {
          bool projectileIntersectsWithBounds = viewport.IntersectsWith(projectile.Bounds);
          bool projectileInsideBounds = viewport.Contains(projectile.Bounds);
          projectile.IsAlive = projectile.StayAlive || (projectileInsideBounds || projectileIntersectsWithBounds);
        }

        // if the projectile is still alive, check to see if it hit anything
        if (projectile.IsAlive)
        {
          CheckCollisions(projectile, polygon, uparams);
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
        switch (dparams.FillMode)
        {
          case GridFillMode.Polygons:
            {
              Polygon hull = piece.GetHull(dparams.Offset);
              dparams.Graphics.DrawPolygon(hull.Points, Color.Blue);
              break;
            }
          case GridFillMode.Solid:
            {
              piece.Draw(dparams);
              break;
            }
        }
      }
      mSelection.Draw(dparams);
    }

    private void DrawGrid(DrawParams dparams)
    {
      Vector2 offset = dparams.Offset;
      SpriteBatch spriteBatch = dparams.SpriteBatch;

      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          GridCell cell = Grid[c, r];
          BoxF bounds = new BoxF(cell.X + offset.X, cell.Y + offset.Y, cell.Width, cell.Height);

          if (cellsToFlash.ContainsKey(cell))
          {
            // draw the flashing cells
            int index = ((int)((SecondsToFlashCell - cellsToFlash[cell]) * FlashesPerSecond)) % MaxFlashes;
            float mu = ((float)index) / ((float)MaxFlashes);
            Color color = ColorHelper.Blend(FlashStart, FlashEnd, mu);
            color = ColorHelper.NewAlpha(color, 200);
            dparams.Graphics.FillRectangle(bounds, color);
          }

          if (cell.IsOuter && !cell.IsThroughway)
          {
            switch (dparams.FillMode)
            {
              case GridFillMode.Solid:
                {
                  dparams.Graphics.FillRectangle(bounds, Color.White);
                  break;
                }
              case GridFillMode.Polygons:
                {
                  dparams.Graphics.DrawRectangle(bounds, Color.White);
                  break;
                }
            }
          }
        }
      }
    }

    private void DrawInvaders(DrawParams dparams)
    {
      foreach (Invader invader in mInvaders)
      {
        switch (dparams.FillMode)
        {
          case GridFillMode.Polygons:
            {
              Polygon hull = invader.GetHull(dparams.Offset);
              dparams.Graphics.DrawPolygon(hull.Points, Color.Yellow);
              break;
            }
          case GridFillMode.Solid:
            {
              invader.Draw(dparams);
              break;
            }
        }
      }
    }

    private void DrawProjectiles(DrawParams dparams)
    {
      foreach (Projectile projectile in mProjectiles)
      {
        switch (dparams.FillMode)
        {
          case GridFillMode.Polygons:
            {
              Polygon hull = projectile.GetHull(dparams.Offset);
              dparams.Graphics.DrawPolygon(hull.Points, Color.Red);
              break;
            }
          case GridFillMode.Solid:
            {
              projectile.Draw(dparams);
              break;
            }
        }
      }
    }

    private void DrawPieceRadius(DrawParams dparams)
    {
      if (!currentArea.IsEmpty)
      {
        Vector2 pos = currentArea.Location + dparams.Offset;
        Vector2 size = new Vector2(currentArea.Width, currentArea.Height);

        dparams.Graphics.FillEllipse(pos, size, ColorHelper.NewAlpha(Color.SlateBlue, .5f));
        dparams.Graphics.DrawEllipse(pos, size, Color.Gold);
      }
    }

    #endregion

    #endregion
  }
}
