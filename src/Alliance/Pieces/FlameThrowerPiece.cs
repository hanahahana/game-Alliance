using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Helpers;
using Alliance.Projectiles;
using Alliance.Data;
using Microsoft.Xna.Framework;
using Alliance.Utilities;

namespace Alliance.Pieces
{
  public class FlameThrowerPiece : Piece
  {
    private const string FlameThrowerPieceName = "Flame Thrower";
    private const string UltimateFlameThrowerPieceName = "Flamewave";

    private string mDescription;
    private float mRadius;
    private float mAttack;
    private int mPrice;
    private int mUpgradePercent;
    private bool mFaceTarget = true;

    public FlameThrowerPiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Ignites gasoline to produce a constant flame. Careful, some enemies can't be burned.");
      mDescription = sb.ToString();

      mRadius = 70;
      mAttack = 9000000;

      mNumberProjectilesToFire = 1;
      mUpgradePercent = 20;
      mPrice = 10;

      mProjectilesPerSecond = 20;
      mProjectileLifeInSeconds = .0876f;
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return FlameThrowerPieceName; }
    }

    public override string UltimateName
    {
      get { return UltimateFlameThrowerPieceName; }
    }

    public override PieceGrouping Grouping
    {
      get { return PieceGrouping.Four; }
    }

    public override float Radius
    {
      get { return mRadius; }
      protected set { ; }
    }

    public override float Attack
    {
      get { return mAttack; }
      protected set { mAttack = value; }
    }

    public override bool FaceTarget
    {
      get { return mFaceTarget; }
    }

    public override int Price
    {
      get { return mPrice; }
      protected set { mPrice = value; }
    }

    public override int UpgradePercent
    {
      get { return mUpgradePercent; }
    }

    protected override Texture2D GetTowerImage()
    {
      return AllianceGame.Textures["flamethrower"];
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      if (mLevel == MaxLevel)
      {
        mOrientation = Environment.TickCount / 1000f;
        mOrientation = Utils.WrapAngle(mOrientation);
      }
    }

    protected override void FinalizeUpgrade()
    {
      base.FinalizeUpgrade();
      if (mLevel == MaxLevel)
      {
        mOrientation = 0;
        mFaceTarget = false;

        mNumberProjectilesToFire = 3;
        mProjectileLifeInSeconds = DefaultProjectileLifeInSeconds * 2;
        mRadius = 100;

        mProjectilesPerSecond = 10;
        mProjectileLifeInSeconds = .1676f;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Rotates to gain energy and releases a VERY powerful flame wave. Careful, some enemies can't be burned.");
        mDescription = sb.ToString();
      }
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      
    }

    protected override Piece CreatePiece(Cell[] cells)
    {
      FlameThrowerPiece piece = new FlameThrowerPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      Projectile projectile = null;
      if (mLevel == MaxLevel)
      {
        BoxF bounds = new BoxF(this.Position, this.Size);
        projectile = new FlamewaveProjectile(bounds, mProjectileLifeInSeconds);
        projectile.Size = new SizeF(Width, Height);
      }
      else
      {
        projectile = new FlameProjectile(mProjectileLifeInSeconds);
      }
      return projectile;
    }
  }
}
