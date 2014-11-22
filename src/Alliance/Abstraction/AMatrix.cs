using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public struct AMatrix : IEquatable<AMatrix>
  {
    public float M11;
    public float M12;
    public float M13;
    public float M14;
    public float M21;
    public float M22;
    public float M23;
    public float M24;
    public float M31;
    public float M32;
    public float M33;
    public float M34;
    public float M41;
    public float M42;
    public float M43;
    public float M44;

    public static AMatrix Identity
    {
      get { return new AMatrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f); }
    }

    public AMatrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24,
                          float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
    {
      this.M11 = m11;
      this.M12 = m12;
      this.M13 = m13;
      this.M14 = m14;
      this.M21 = m21;
      this.M22 = m22;
      this.M23 = m23;
      this.M24 = m24;
      this.M31 = m31;
      this.M32 = m32;
      this.M33 = m33;
      this.M34 = m34;
      this.M41 = m41;
      this.M42 = m42;
      this.M43 = m43;
      this.M44 = m44;
    }

    public static AMatrix Add(AMatrix matrix1, AMatrix matrix2)
    {
      matrix1.M11 += matrix2.M11;
      matrix1.M12 += matrix2.M12;
      matrix1.M13 += matrix2.M13;
      matrix1.M14 += matrix2.M14;
      matrix1.M21 += matrix2.M21;
      matrix1.M22 += matrix2.M22;
      matrix1.M23 += matrix2.M23;
      matrix1.M24 += matrix2.M24;
      matrix1.M31 += matrix2.M31;
      matrix1.M32 += matrix2.M32;
      matrix1.M33 += matrix2.M33;
      matrix1.M34 += matrix2.M34;
      matrix1.M41 += matrix2.M41;
      matrix1.M42 += matrix2.M42;
      matrix1.M43 += matrix2.M43;
      matrix1.M44 += matrix2.M44;
      return matrix1;
    }

    public static void Add(ref AMatrix matrix1, ref AMatrix matrix2, out AMatrix result)
    {
      result.M11 = matrix1.M11 + matrix2.M11;
      result.M12 = matrix1.M12 + matrix2.M12;
      result.M13 = matrix1.M13 + matrix2.M13;
      result.M14 = matrix1.M14 + matrix2.M14;
      result.M21 = matrix1.M21 + matrix2.M21;
      result.M22 = matrix1.M22 + matrix2.M22;
      result.M23 = matrix1.M23 + matrix2.M23;
      result.M24 = matrix1.M24 + matrix2.M24;
      result.M31 = matrix1.M31 + matrix2.M31;
      result.M32 = matrix1.M32 + matrix2.M32;
      result.M33 = matrix1.M33 + matrix2.M33;
      result.M34 = matrix1.M34 + matrix2.M34;
      result.M41 = matrix1.M41 + matrix2.M41;
      result.M42 = matrix1.M42 + matrix2.M42;
      result.M43 = matrix1.M43 + matrix2.M43;
      result.M44 = matrix1.M44 + matrix2.M44;
    }

    public static AMatrix CreateRotationX(float radians)
    {
      AMatrix returnMatrix = AMatrix.Identity;

      returnMatrix.M22 = (float)Math.Cos(radians);
      returnMatrix.M23 = (float)Math.Sin(radians);
      returnMatrix.M32 = -returnMatrix.M23;
      returnMatrix.M33 = returnMatrix.M22;

      return returnMatrix;
    }

    public static void CreateRotationX(float radians, out AMatrix result)
    {
      result = AMatrix.Identity;

      result.M22 = (float)Math.Cos(radians);
      result.M23 = (float)Math.Sin(radians);
      result.M32 = -result.M23;
      result.M33 = result.M22;
    }

    public static AMatrix CreateRotationY(float radians)
    {
      AMatrix returnMatrix = AMatrix.Identity;

      returnMatrix.M11 = (float)Math.Cos(radians);
      returnMatrix.M13 = (float)Math.Sin(radians);
      returnMatrix.M31 = -returnMatrix.M13;
      returnMatrix.M33 = returnMatrix.M11;

      return returnMatrix;
    }

    public static void CreateRotationY(float radians, out AMatrix result)
    {
      result = AMatrix.Identity;

      result.M11 = (float)Math.Cos(radians);
      result.M13 = (float)Math.Sin(radians);
      result.M31 = -result.M13;
      result.M33 = result.M11;
    }

    public static AMatrix CreateRotationZ(float radians)
    {
      AMatrix returnMatrix = AMatrix.Identity;

      returnMatrix.M11 = (float)Math.Cos(radians);
      returnMatrix.M12 = (float)Math.Sin(radians);
      returnMatrix.M21 = -returnMatrix.M12;
      returnMatrix.M22 = returnMatrix.M11;

      return returnMatrix;
    }

    public static void CreateRotationZ(float radians, out AMatrix result)
    {
      result = AMatrix.Identity;

      result.M11 = (float)Math.Cos(radians);
      result.M12 = (float)Math.Sin(radians);
      result.M21 = -result.M12;
      result.M22 = result.M11;
    }

    public static AMatrix CreateScale(float scale)
    {
      AMatrix returnMatrix = AMatrix.Identity;

      returnMatrix.M11 = scale;
      returnMatrix.M22 = scale;
      returnMatrix.M33 = scale;

      return returnMatrix;
    }

    public static void CreateScale(float scale, out AMatrix result)
    {
      result = AMatrix.Identity;

      result.M11 = scale;
      result.M22 = scale;
      result.M33 = scale;
    }

    public static AMatrix CreateScale(float xScale, float yScale, float zScale)
    {
      AMatrix returnMatrix = AMatrix.Identity;

      returnMatrix.M11 = xScale;
      returnMatrix.M22 = yScale;
      returnMatrix.M33 = zScale;

      return returnMatrix;
    }

    public static void CreateScale(float xScale, float yScale, float zScale, out AMatrix result)
    {
      result = AMatrix.Identity;

      result.M11 = xScale;
      result.M22 = yScale;
      result.M33 = zScale;
    }

    public static AMatrix CreateTranslation(float xPosition, float yPosition, float zPosition)
    {
      AMatrix returnMatrix = AMatrix.Identity;

      returnMatrix.M41 = xPosition;
      returnMatrix.M42 = yPosition;
      returnMatrix.M43 = zPosition;

      return returnMatrix;
    }

    public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out AMatrix result)
    {
      result = AMatrix.Identity;

      result.M41 = xPosition;
      result.M42 = yPosition;
      result.M43 = zPosition;
    }

    public static AMatrix Divide(AMatrix matrix1, AMatrix matrix2)
    {
      AMatrix inverse = AMatrix.Invert(matrix2), result;

      result.M11 = matrix1.M11 * inverse.M11 + matrix1.M12 * inverse.M21 + matrix1.M13 * inverse.M31 + matrix1.M14 * inverse.M41;
      result.M12 = matrix1.M11 * inverse.M12 + matrix1.M12 * inverse.M22 + matrix1.M13 * inverse.M32 + matrix1.M14 * inverse.M42;
      result.M13 = matrix1.M11 * inverse.M13 + matrix1.M12 * inverse.M23 + matrix1.M13 * inverse.M33 + matrix1.M14 * inverse.M43;
      result.M14 = matrix1.M11 * inverse.M14 + matrix1.M12 * inverse.M24 + matrix1.M13 * inverse.M34 + matrix1.M14 * inverse.M44;

      result.M21 = matrix1.M21 * inverse.M11 + matrix1.M22 * inverse.M21 + matrix1.M23 * inverse.M31 + matrix1.M24 * inverse.M41;
      result.M22 = matrix1.M21 * inverse.M12 + matrix1.M22 * inverse.M22 + matrix1.M23 * inverse.M32 + matrix1.M24 * inverse.M42;
      result.M23 = matrix1.M21 * inverse.M13 + matrix1.M22 * inverse.M23 + matrix1.M23 * inverse.M33 + matrix1.M24 * inverse.M43;
      result.M24 = matrix1.M21 * inverse.M14 + matrix1.M22 * inverse.M24 + matrix1.M23 * inverse.M34 + matrix1.M24 * inverse.M44;

      result.M31 = matrix1.M31 * inverse.M11 + matrix1.M32 * inverse.M21 + matrix1.M33 * inverse.M31 + matrix1.M34 * inverse.M41;
      result.M32 = matrix1.M31 * inverse.M12 + matrix1.M32 * inverse.M22 + matrix1.M33 * inverse.M32 + matrix1.M34 * inverse.M42;
      result.M33 = matrix1.M31 * inverse.M13 + matrix1.M32 * inverse.M23 + matrix1.M33 * inverse.M33 + matrix1.M34 * inverse.M43;
      result.M34 = matrix1.M31 * inverse.M14 + matrix1.M32 * inverse.M24 + matrix1.M33 * inverse.M34 + matrix1.M34 * inverse.M44;

      result.M41 = matrix1.M41 * inverse.M11 + matrix1.M42 * inverse.M21 + matrix1.M43 * inverse.M31 + matrix1.M44 * inverse.M41;
      result.M42 = matrix1.M41 * inverse.M12 + matrix1.M42 * inverse.M22 + matrix1.M43 * inverse.M32 + matrix1.M44 * inverse.M42;
      result.M43 = matrix1.M41 * inverse.M13 + matrix1.M42 * inverse.M23 + matrix1.M43 * inverse.M33 + matrix1.M44 * inverse.M43;
      result.M44 = matrix1.M41 * inverse.M14 + matrix1.M42 * inverse.M24 + matrix1.M43 * inverse.M34 + matrix1.M44 * inverse.M44;

      return result;
    }

    public static void Divide(ref AMatrix matrix1, ref AMatrix matrix2, out AMatrix result)
    {
      AMatrix inverse = AMatrix.Invert(matrix2);
      result.M11 = matrix1.M11 * inverse.M11 + matrix1.M12 * inverse.M21 + matrix1.M13 * inverse.M31 + matrix1.M14 * inverse.M41;
      result.M12 = matrix1.M11 * inverse.M12 + matrix1.M12 * inverse.M22 + matrix1.M13 * inverse.M32 + matrix1.M14 * inverse.M42;
      result.M13 = matrix1.M11 * inverse.M13 + matrix1.M12 * inverse.M23 + matrix1.M13 * inverse.M33 + matrix1.M14 * inverse.M43;
      result.M14 = matrix1.M11 * inverse.M14 + matrix1.M12 * inverse.M24 + matrix1.M13 * inverse.M34 + matrix1.M14 * inverse.M44;

      result.M21 = matrix1.M21 * inverse.M11 + matrix1.M22 * inverse.M21 + matrix1.M23 * inverse.M31 + matrix1.M24 * inverse.M41;
      result.M22 = matrix1.M21 * inverse.M12 + matrix1.M22 * inverse.M22 + matrix1.M23 * inverse.M32 + matrix1.M24 * inverse.M42;
      result.M23 = matrix1.M21 * inverse.M13 + matrix1.M22 * inverse.M23 + matrix1.M23 * inverse.M33 + matrix1.M24 * inverse.M43;
      result.M24 = matrix1.M21 * inverse.M14 + matrix1.M22 * inverse.M24 + matrix1.M23 * inverse.M34 + matrix1.M24 * inverse.M44;

      result.M31 = matrix1.M31 * inverse.M11 + matrix1.M32 * inverse.M21 + matrix1.M33 * inverse.M31 + matrix1.M34 * inverse.M41;
      result.M32 = matrix1.M31 * inverse.M12 + matrix1.M32 * inverse.M22 + matrix1.M33 * inverse.M32 + matrix1.M34 * inverse.M42;
      result.M33 = matrix1.M31 * inverse.M13 + matrix1.M32 * inverse.M23 + matrix1.M33 * inverse.M33 + matrix1.M34 * inverse.M43;
      result.M34 = matrix1.M31 * inverse.M14 + matrix1.M32 * inverse.M24 + matrix1.M33 * inverse.M34 + matrix1.M34 * inverse.M44;

      result.M41 = matrix1.M41 * inverse.M11 + matrix1.M42 * inverse.M21 + matrix1.M43 * inverse.M31 + matrix1.M44 * inverse.M41;
      result.M42 = matrix1.M41 * inverse.M12 + matrix1.M42 * inverse.M22 + matrix1.M43 * inverse.M32 + matrix1.M44 * inverse.M42;
      result.M43 = matrix1.M41 * inverse.M13 + matrix1.M42 * inverse.M23 + matrix1.M43 * inverse.M33 + matrix1.M44 * inverse.M43;
      result.M44 = matrix1.M41 * inverse.M14 + matrix1.M42 * inverse.M24 + matrix1.M43 * inverse.M34 + matrix1.M44 * inverse.M44;
    }

    public static AMatrix Divide(AMatrix matrix1, float divider)
    {
      float inverseDivider = 1.0f / divider;

      matrix1.M11 = matrix1.M11 * inverseDivider;
      matrix1.M12 = matrix1.M12 * inverseDivider;
      matrix1.M13 = matrix1.M13 * inverseDivider;
      matrix1.M14 = matrix1.M14 * inverseDivider;
      matrix1.M21 = matrix1.M21 * inverseDivider;
      matrix1.M22 = matrix1.M22 * inverseDivider;
      matrix1.M23 = matrix1.M23 * inverseDivider;
      matrix1.M24 = matrix1.M24 * inverseDivider;
      matrix1.M31 = matrix1.M31 * inverseDivider;
      matrix1.M32 = matrix1.M32 * inverseDivider;
      matrix1.M33 = matrix1.M33 * inverseDivider;
      matrix1.M34 = matrix1.M34 * inverseDivider;
      matrix1.M41 = matrix1.M41 * inverseDivider;
      matrix1.M42 = matrix1.M42 * inverseDivider;
      matrix1.M43 = matrix1.M43 * inverseDivider;
      matrix1.M44 = matrix1.M44 * inverseDivider;

      return matrix1;
    }

    public static void Divide(ref AMatrix matrix1, float divider, out AMatrix result)
    {
      float inverseDivider = 1.0f / divider;
      result.M11 = matrix1.M11 * inverseDivider;
      result.M12 = matrix1.M12 * inverseDivider;
      result.M13 = matrix1.M13 * inverseDivider;
      result.M14 = matrix1.M14 * inverseDivider;
      result.M21 = matrix1.M21 * inverseDivider;
      result.M22 = matrix1.M22 * inverseDivider;
      result.M23 = matrix1.M23 * inverseDivider;
      result.M24 = matrix1.M24 * inverseDivider;
      result.M31 = matrix1.M31 * inverseDivider;
      result.M32 = matrix1.M32 * inverseDivider;
      result.M33 = matrix1.M33 * inverseDivider;
      result.M34 = matrix1.M34 * inverseDivider;
      result.M41 = matrix1.M41 * inverseDivider;
      result.M42 = matrix1.M42 * inverseDivider;
      result.M43 = matrix1.M43 * inverseDivider;
      result.M44 = matrix1.M44 * inverseDivider;
    }

    public static AMatrix Invert(AMatrix matrix)
    {
      Invert(ref matrix, out matrix);
      return matrix;
    }

    public static void Invert(ref AMatrix matrix, out AMatrix result)
    {
      //
      // Use Laplace expansion theorem to calculate the inverse of a 4x4 matrix
      // 
      // 1. Calculate the 2x2 determinants needed and the 4x4 determinant based on the 2x2 determinants 
      // 2. Create the adjugate matrix, which satisfies: A * adj(A) = det(A) * I
      // 3. Divide adjugate matrix with the determinant to find the inverse

      float det1 = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
      float det2 = matrix.M11 * matrix.M23 - matrix.M13 * matrix.M21;
      float det3 = matrix.M11 * matrix.M24 - matrix.M14 * matrix.M21;
      float det4 = matrix.M12 * matrix.M23 - matrix.M13 * matrix.M22;
      float det5 = matrix.M12 * matrix.M24 - matrix.M14 * matrix.M22;
      float det6 = matrix.M13 * matrix.M24 - matrix.M14 * matrix.M23;
      float det7 = matrix.M31 * matrix.M42 - matrix.M32 * matrix.M41;
      float det8 = matrix.M31 * matrix.M43 - matrix.M33 * matrix.M41;
      float det9 = matrix.M31 * matrix.M44 - matrix.M34 * matrix.M41;
      float det10 = matrix.M32 * matrix.M43 - matrix.M33 * matrix.M42;
      float det11 = matrix.M32 * matrix.M44 - matrix.M34 * matrix.M42;
      float det12 = matrix.M33 * matrix.M44 - matrix.M34 * matrix.M43;

      float detMatrix = (float)(det1 * det12 - det2 * det11 + det3 * det10 + det4 * det9 - det5 * det8 + det6 * det7);

      float invDetMatrix = 1f / detMatrix;

      AMatrix ret; // Allow for matrix and result to point to the same structure

      ret.M11 = (matrix.M22 * det12 - matrix.M23 * det11 + matrix.M24 * det10) * invDetMatrix;
      ret.M12 = (-matrix.M12 * det12 + matrix.M13 * det11 - matrix.M14 * det10) * invDetMatrix;
      ret.M13 = (matrix.M42 * det6 - matrix.M43 * det5 + matrix.M44 * det4) * invDetMatrix;
      ret.M14 = (-matrix.M32 * det6 + matrix.M33 * det5 - matrix.M34 * det4) * invDetMatrix;
      ret.M21 = (-matrix.M21 * det12 + matrix.M23 * det9 - matrix.M24 * det8) * invDetMatrix;
      ret.M22 = (matrix.M11 * det12 - matrix.M13 * det9 + matrix.M14 * det8) * invDetMatrix;
      ret.M23 = (-matrix.M41 * det6 + matrix.M43 * det3 - matrix.M44 * det2) * invDetMatrix;
      ret.M24 = (matrix.M31 * det6 - matrix.M33 * det3 + matrix.M34 * det2) * invDetMatrix;
      ret.M31 = (matrix.M21 * det11 - matrix.M22 * det9 + matrix.M24 * det7) * invDetMatrix;
      ret.M32 = (-matrix.M11 * det11 + matrix.M12 * det9 - matrix.M14 * det7) * invDetMatrix;
      ret.M33 = (matrix.M41 * det5 - matrix.M42 * det3 + matrix.M44 * det1) * invDetMatrix;
      ret.M34 = (-matrix.M31 * det5 + matrix.M32 * det3 - matrix.M34 * det1) * invDetMatrix;
      ret.M41 = (-matrix.M21 * det10 + matrix.M22 * det8 - matrix.M23 * det7) * invDetMatrix;
      ret.M42 = (matrix.M11 * det10 - matrix.M12 * det8 + matrix.M13 * det7) * invDetMatrix;
      ret.M43 = (-matrix.M41 * det4 + matrix.M42 * det2 - matrix.M43 * det1) * invDetMatrix;
      ret.M44 = (matrix.M31 * det4 - matrix.M32 * det2 + matrix.M33 * det1) * invDetMatrix;

      result = ret;
    }

    public static AMatrix Multiply(AMatrix matrix1, AMatrix matrix2)
    {
      AMatrix result;

      result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
      result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
      result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
      result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;

      result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
      result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
      result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
      result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;

      result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
      result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
      result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
      result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;

      result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
      result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
      result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
      result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;

      return result;
    }

    public static void Multiply(ref AMatrix matrix1, ref AMatrix matrix2, out AMatrix result)
    {
      result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
      result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
      result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
      result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;

      result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
      result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
      result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
      result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;

      result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
      result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
      result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
      result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;

      result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
      result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
      result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
      result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
    }

    public static AMatrix Multiply(AMatrix matrix1, float factor)
    {
      matrix1.M11 *= factor;
      matrix1.M12 *= factor;
      matrix1.M13 *= factor;
      matrix1.M14 *= factor;
      matrix1.M21 *= factor;
      matrix1.M22 *= factor;
      matrix1.M23 *= factor;
      matrix1.M24 *= factor;
      matrix1.M31 *= factor;
      matrix1.M32 *= factor;
      matrix1.M33 *= factor;
      matrix1.M34 *= factor;
      matrix1.M41 *= factor;
      matrix1.M42 *= factor;
      matrix1.M43 *= factor;
      matrix1.M44 *= factor;
      return matrix1;
    }

    public static void Multiply(ref AMatrix matrix1, float factor, out AMatrix result)
    {
      result.M11 = matrix1.M11 * factor;
      result.M12 = matrix1.M12 * factor;
      result.M13 = matrix1.M13 * factor;
      result.M14 = matrix1.M14 * factor;
      result.M21 = matrix1.M21 * factor;
      result.M22 = matrix1.M22 * factor;
      result.M23 = matrix1.M23 * factor;
      result.M24 = matrix1.M24 * factor;
      result.M31 = matrix1.M31 * factor;
      result.M32 = matrix1.M32 * factor;
      result.M33 = matrix1.M33 * factor;
      result.M34 = matrix1.M34 * factor;
      result.M41 = matrix1.M41 * factor;
      result.M42 = matrix1.M42 * factor;
      result.M43 = matrix1.M43 * factor;
      result.M44 = matrix1.M44 * factor;
    }

    public static AMatrix Negate(AMatrix matrix)
    {
      matrix.M11 = -matrix.M11;
      matrix.M12 = -matrix.M12;
      matrix.M13 = -matrix.M13;
      matrix.M14 = -matrix.M14;
      matrix.M21 = -matrix.M21;
      matrix.M22 = -matrix.M22;
      matrix.M23 = -matrix.M23;
      matrix.M24 = -matrix.M24;
      matrix.M31 = -matrix.M31;
      matrix.M32 = -matrix.M32;
      matrix.M33 = -matrix.M33;
      matrix.M34 = -matrix.M34;
      matrix.M41 = -matrix.M41;
      matrix.M42 = -matrix.M42;
      matrix.M43 = -matrix.M43;
      matrix.M44 = -matrix.M44;
      return matrix;
    }

    public static void Negate(ref AMatrix matrix, out AMatrix result)
    {
      result.M11 = matrix.M11;
      result.M12 = matrix.M12;
      result.M13 = matrix.M13;
      result.M14 = matrix.M14;
      result.M21 = matrix.M21;
      result.M22 = matrix.M22;
      result.M23 = matrix.M23;
      result.M24 = matrix.M24;
      result.M31 = matrix.M31;
      result.M32 = matrix.M32;
      result.M33 = matrix.M33;
      result.M34 = matrix.M34;
      result.M41 = matrix.M41;
      result.M42 = matrix.M42;
      result.M43 = matrix.M43;
      result.M44 = matrix.M44;
    }

    public static AMatrix Subtract(AMatrix matrix1, AMatrix matrix2)
    {
      matrix1.M11 -= matrix2.M11;
      matrix1.M12 -= matrix2.M12;
      matrix1.M13 -= matrix2.M13;
      matrix1.M14 -= matrix2.M14;
      matrix1.M21 -= matrix2.M21;
      matrix1.M22 -= matrix2.M22;
      matrix1.M23 -= matrix2.M23;
      matrix1.M24 -= matrix2.M24;
      matrix1.M31 -= matrix2.M31;
      matrix1.M32 -= matrix2.M32;
      matrix1.M33 -= matrix2.M33;
      matrix1.M34 -= matrix2.M34;
      matrix1.M41 -= matrix2.M41;
      matrix1.M42 -= matrix2.M42;
      matrix1.M43 -= matrix2.M43;
      matrix1.M44 -= matrix2.M44;
      return matrix1;
    }

    public static void Subtract(ref AMatrix matrix1, ref AMatrix matrix2, out AMatrix result)
    {
      result.M11 = matrix1.M11 - matrix2.M11;
      result.M12 = matrix1.M12 - matrix2.M12;
      result.M13 = matrix1.M13 - matrix2.M13;
      result.M14 = matrix1.M14 - matrix2.M14;
      result.M21 = matrix1.M21 - matrix2.M21;
      result.M22 = matrix1.M22 - matrix2.M22;
      result.M23 = matrix1.M23 - matrix2.M23;
      result.M24 = matrix1.M24 - matrix2.M24;
      result.M31 = matrix1.M31 - matrix2.M31;
      result.M32 = matrix1.M32 - matrix2.M32;
      result.M33 = matrix1.M33 - matrix2.M33;
      result.M34 = matrix1.M34 - matrix2.M34;
      result.M41 = matrix1.M41 - matrix2.M41;
      result.M42 = matrix1.M42 - matrix2.M42;
      result.M43 = matrix1.M43 - matrix2.M43;
      result.M44 = matrix1.M44 - matrix2.M44;
    }

    public static AMatrix Transpose(AMatrix matrix)
    {
      AMatrix result;

      result.M11 = matrix.M11;
      result.M12 = matrix.M21;
      result.M13 = matrix.M31;
      result.M14 = matrix.M41;

      result.M21 = matrix.M12;
      result.M22 = matrix.M22;
      result.M23 = matrix.M32;
      result.M24 = matrix.M42;

      result.M31 = matrix.M13;
      result.M32 = matrix.M23;
      result.M33 = matrix.M33;
      result.M34 = matrix.M43;

      result.M41 = matrix.M14;
      result.M42 = matrix.M24;
      result.M43 = matrix.M34;
      result.M44 = matrix.M44;

      return result;
    }

    public static void Transpose(ref AMatrix matrix, out AMatrix result)
    {
      result.M11 = matrix.M11;
      result.M12 = matrix.M21;
      result.M13 = matrix.M31;
      result.M14 = matrix.M41;

      result.M21 = matrix.M12;
      result.M22 = matrix.M22;
      result.M23 = matrix.M32;
      result.M24 = matrix.M42;

      result.M31 = matrix.M13;
      result.M32 = matrix.M23;
      result.M33 = matrix.M33;
      result.M34 = matrix.M43;

      result.M41 = matrix.M14;
      result.M42 = matrix.M24;
      result.M43 = matrix.M34;
      result.M44 = matrix.M44;
    }

    public float Determinant()
    {
      float minor1, minor2, minor3, minor4, minor5, minor6;

      minor1 = M31 * M42 - M32 * M41;
      minor2 = M31 * M43 - M33 * M41;
      minor3 = M31 * M44 - M34 * M41;
      minor4 = M32 * M43 - M33 * M42;
      minor5 = M32 * M44 - M34 * M42;
      minor6 = M33 * M44 - M34 * M43;

      return M11 * (M22 * minor6 - M23 * minor5 + M24 * minor4) -
                      M12 * (M21 * minor6 - M23 * minor3 + M24 * minor2) +
                      M13 * (M21 * minor5 - M22 * minor3 + M24 * minor1) -
                      M14 * (M21 * minor4 - M22 * minor2 + M23 * minor1);
    }

    public bool Equals(AMatrix other)
    {
      return (this.M11 == other.M11) && (this.M12 == other.M12) &&
             (this.M13 == other.M13) && (this.M14 == other.M14) &&
             (this.M21 == other.M21) && (this.M22 == other.M22) &&
             (this.M23 == other.M23) && (this.M24 == other.M24) &&
             (this.M31 == other.M31) && (this.M32 == other.M32) &&
             (this.M33 == other.M33) && (this.M34 == other.M34) &&
             (this.M41 == other.M41) && (this.M42 == other.M42) &&
             (this.M43 == other.M43) && (this.M44 == other.M44);
    }

    public static AMatrix operator +(AMatrix matrix1, AMatrix matrix2)
    {
      matrix1.M11 += matrix2.M11;
      matrix1.M12 += matrix2.M12;
      matrix1.M13 += matrix2.M13;
      matrix1.M14 += matrix2.M14;
      matrix1.M21 += matrix2.M21;
      matrix1.M22 += matrix2.M22;
      matrix1.M23 += matrix2.M23;
      matrix1.M24 += matrix2.M24;
      matrix1.M31 += matrix2.M31;
      matrix1.M32 += matrix2.M32;
      matrix1.M33 += matrix2.M33;
      matrix1.M34 += matrix2.M34;
      matrix1.M41 += matrix2.M41;
      matrix1.M42 += matrix2.M42;
      matrix1.M43 += matrix2.M43;
      matrix1.M44 += matrix2.M44;
      return matrix1;
    }

    public static AMatrix operator /(AMatrix matrix1, AMatrix matrix2)
    {
      AMatrix inverse = AMatrix.Invert(matrix2), result;

      result.M11 = matrix1.M11 * inverse.M11 + matrix1.M12 * inverse.M21 + matrix1.M13 * inverse.M31 + matrix1.M14 * inverse.M41;
      result.M12 = matrix1.M11 * inverse.M12 + matrix1.M12 * inverse.M22 + matrix1.M13 * inverse.M32 + matrix1.M14 * inverse.M42;
      result.M13 = matrix1.M11 * inverse.M13 + matrix1.M12 * inverse.M23 + matrix1.M13 * inverse.M33 + matrix1.M14 * inverse.M43;
      result.M14 = matrix1.M11 * inverse.M14 + matrix1.M12 * inverse.M24 + matrix1.M13 * inverse.M34 + matrix1.M14 * inverse.M44;

      result.M21 = matrix1.M21 * inverse.M11 + matrix1.M22 * inverse.M21 + matrix1.M23 * inverse.M31 + matrix1.M24 * inverse.M41;
      result.M22 = matrix1.M21 * inverse.M12 + matrix1.M22 * inverse.M22 + matrix1.M23 * inverse.M32 + matrix1.M24 * inverse.M42;
      result.M23 = matrix1.M21 * inverse.M13 + matrix1.M22 * inverse.M23 + matrix1.M23 * inverse.M33 + matrix1.M24 * inverse.M43;
      result.M24 = matrix1.M21 * inverse.M14 + matrix1.M22 * inverse.M24 + matrix1.M23 * inverse.M34 + matrix1.M24 * inverse.M44;

      result.M31 = matrix1.M31 * inverse.M11 + matrix1.M32 * inverse.M21 + matrix1.M33 * inverse.M31 + matrix1.M34 * inverse.M41;
      result.M32 = matrix1.M31 * inverse.M12 + matrix1.M32 * inverse.M22 + matrix1.M33 * inverse.M32 + matrix1.M34 * inverse.M42;
      result.M33 = matrix1.M31 * inverse.M13 + matrix1.M32 * inverse.M23 + matrix1.M33 * inverse.M33 + matrix1.M34 * inverse.M43;
      result.M34 = matrix1.M31 * inverse.M14 + matrix1.M32 * inverse.M24 + matrix1.M33 * inverse.M34 + matrix1.M34 * inverse.M44;

      result.M41 = matrix1.M41 * inverse.M11 + matrix1.M42 * inverse.M21 + matrix1.M43 * inverse.M31 + matrix1.M44 * inverse.M41;
      result.M42 = matrix1.M41 * inverse.M12 + matrix1.M42 * inverse.M22 + matrix1.M43 * inverse.M32 + matrix1.M44 * inverse.M42;
      result.M43 = matrix1.M41 * inverse.M13 + matrix1.M42 * inverse.M23 + matrix1.M43 * inverse.M33 + matrix1.M44 * inverse.M43;
      result.M44 = matrix1.M41 * inverse.M14 + matrix1.M42 * inverse.M24 + matrix1.M43 * inverse.M34 + matrix1.M44 * inverse.M44;

      return result;
    }

    public static AMatrix operator /(AMatrix matrix1, float divider)
    {
      float inverseDivider = 1.0f / divider;

      matrix1.M11 = matrix1.M11 * inverseDivider;
      matrix1.M12 = matrix1.M12 * inverseDivider;
      matrix1.M13 = matrix1.M13 * inverseDivider;
      matrix1.M14 = matrix1.M14 * inverseDivider;
      matrix1.M21 = matrix1.M21 * inverseDivider;
      matrix1.M22 = matrix1.M22 * inverseDivider;
      matrix1.M23 = matrix1.M23 * inverseDivider;
      matrix1.M24 = matrix1.M24 * inverseDivider;
      matrix1.M31 = matrix1.M31 * inverseDivider;
      matrix1.M32 = matrix1.M32 * inverseDivider;
      matrix1.M33 = matrix1.M33 * inverseDivider;
      matrix1.M34 = matrix1.M34 * inverseDivider;
      matrix1.M41 = matrix1.M41 * inverseDivider;
      matrix1.M42 = matrix1.M42 * inverseDivider;
      matrix1.M43 = matrix1.M43 * inverseDivider;
      matrix1.M44 = matrix1.M44 * inverseDivider;

      return matrix1;
    }

    public static bool operator ==(AMatrix matrix1, AMatrix matrix2)
    {
      return (matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12) &&
             (matrix1.M13 == matrix2.M13) && (matrix1.M14 == matrix2.M14) &&
             (matrix1.M21 == matrix2.M21) && (matrix1.M22 == matrix2.M22) &&
             (matrix1.M23 == matrix2.M23) && (matrix1.M24 == matrix2.M24) &&
             (matrix1.M31 == matrix2.M31) && (matrix1.M32 == matrix2.M32) &&
             (matrix1.M33 == matrix2.M33) && (matrix1.M34 == matrix2.M34) &&
             (matrix1.M41 == matrix2.M41) && (matrix1.M42 == matrix2.M42) &&
             (matrix1.M43 == matrix2.M43) && (matrix1.M44 == matrix2.M44);
    }

    public static bool operator !=(AMatrix matrix1, AMatrix matrix2)
    {
      return (matrix1.M11 != matrix2.M11) || (matrix1.M12 != matrix2.M12) ||
             (matrix1.M13 != matrix2.M13) || (matrix1.M14 != matrix2.M14) ||
             (matrix1.M21 != matrix2.M21) || (matrix1.M22 != matrix2.M22) ||
             (matrix1.M23 != matrix2.M23) || (matrix1.M24 != matrix2.M24) ||
             (matrix1.M31 != matrix2.M31) || (matrix1.M32 != matrix2.M32) ||
             (matrix1.M33 != matrix2.M33) || (matrix1.M34 != matrix2.M34) ||
             (matrix1.M41 != matrix2.M41) || (matrix1.M42 != matrix2.M42) ||
             (matrix1.M43 != matrix2.M43) || (matrix1.M44 != matrix2.M44);
    }

    public static AMatrix operator *(AMatrix matrix1, AMatrix matrix2)
    {
      AMatrix result;

      result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
      result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
      result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
      result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;

      result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
      result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
      result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
      result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;

      result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
      result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
      result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
      result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;

      result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
      result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
      result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
      result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;

      return result;
    }

    public static AMatrix operator *(AMatrix matrix, float scaleFactor)
    {
      matrix.M11 = matrix.M11 * scaleFactor;
      matrix.M12 = matrix.M12 * scaleFactor;
      matrix.M13 = matrix.M13 * scaleFactor;
      matrix.M14 = matrix.M14 * scaleFactor;
      matrix.M21 = matrix.M21 * scaleFactor;
      matrix.M22 = matrix.M22 * scaleFactor;
      matrix.M23 = matrix.M23 * scaleFactor;
      matrix.M24 = matrix.M24 * scaleFactor;
      matrix.M31 = matrix.M31 * scaleFactor;
      matrix.M32 = matrix.M32 * scaleFactor;
      matrix.M33 = matrix.M33 * scaleFactor;
      matrix.M34 = matrix.M34 * scaleFactor;
      matrix.M41 = matrix.M41 * scaleFactor;
      matrix.M42 = matrix.M42 * scaleFactor;
      matrix.M43 = matrix.M43 * scaleFactor;
      matrix.M44 = matrix.M44 * scaleFactor;
      return matrix;
    }

    public static AMatrix operator *(float scaleFactor, AMatrix matrix)
    {
      matrix.M11 = matrix.M11 * scaleFactor;
      matrix.M12 = matrix.M12 * scaleFactor;
      matrix.M13 = matrix.M13 * scaleFactor;
      matrix.M14 = matrix.M14 * scaleFactor;
      matrix.M21 = matrix.M21 * scaleFactor;
      matrix.M22 = matrix.M22 * scaleFactor;
      matrix.M23 = matrix.M23 * scaleFactor;
      matrix.M24 = matrix.M24 * scaleFactor;
      matrix.M31 = matrix.M31 * scaleFactor;
      matrix.M32 = matrix.M32 * scaleFactor;
      matrix.M33 = matrix.M33 * scaleFactor;
      matrix.M34 = matrix.M34 * scaleFactor;
      matrix.M41 = matrix.M41 * scaleFactor;
      matrix.M42 = matrix.M42 * scaleFactor;
      matrix.M43 = matrix.M43 * scaleFactor;
      matrix.M44 = matrix.M44 * scaleFactor;
      return matrix;
    }

    public static AMatrix operator -(AMatrix matrix1, AMatrix matrix2)
    {
      matrix1.M11 -= matrix2.M11;
      matrix1.M12 -= matrix2.M12;
      matrix1.M13 -= matrix2.M13;
      matrix1.M14 -= matrix2.M14;
      matrix1.M21 -= matrix2.M21;
      matrix1.M22 -= matrix2.M22;
      matrix1.M23 -= matrix2.M23;
      matrix1.M24 -= matrix2.M24;
      matrix1.M31 -= matrix2.M31;
      matrix1.M32 -= matrix2.M32;
      matrix1.M33 -= matrix2.M33;
      matrix1.M34 -= matrix2.M34;
      matrix1.M41 -= matrix2.M41;
      matrix1.M42 -= matrix2.M42;
      matrix1.M43 -= matrix2.M43;
      matrix1.M44 -= matrix2.M44;
      return matrix1;
    }

    public static AMatrix operator -(AMatrix matrix)
    {
      matrix.M11 = -matrix.M11;
      matrix.M12 = -matrix.M12;
      matrix.M13 = -matrix.M13;
      matrix.M14 = -matrix.M14;
      matrix.M21 = -matrix.M21;
      matrix.M22 = -matrix.M22;
      matrix.M23 = -matrix.M23;
      matrix.M24 = -matrix.M24;
      matrix.M31 = -matrix.M31;
      matrix.M32 = -matrix.M32;
      matrix.M33 = -matrix.M33;
      matrix.M34 = -matrix.M34;
      matrix.M41 = -matrix.M41;
      matrix.M42 = -matrix.M42;
      matrix.M43 = -matrix.M43;
      matrix.M44 = -matrix.M44;
      return matrix;
    }

    public override bool Equals(object obj)
    {
      if (obj is AMatrix)
        return this == (AMatrix)obj;
      return false;
    }

    public override string ToString()
    {
      return "{ {M11:" + M11 + " M12:" + M12 + " M13:" + M13 + " M14:" + M14 + "}" +
              " {M21:" + M21 + " M22:" + M22 + " M23:" + M23 + " M24:" + M24 + "}" +
              " {M31:" + M31 + " M32:" + M32 + " M33:" + M33 + " M34:" + M34 + "}" +
              " {M41:" + M41 + " M42:" + M42 + " M43:" + M43 + " M44:" + M44 + "} }";
    }
  }
}
