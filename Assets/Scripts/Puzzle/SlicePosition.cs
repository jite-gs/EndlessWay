namespace Puzzle
{
#pragma warning disable 661, 659 //GetHashCode
	public struct SlicePosition
	{
		public int X, Y;


		//=== Ctor ============================================================

		public SlicePosition(int x, int y)
		{
			X = x;
			Y = y;
		}

		//=== Public ==========================================================

		public override string ToString()
		{
			return string.Format("SlicePosition({0},{1})", X, Y);
		}

		public bool OnSameLine(SlicePosition otherSlicePosition)
		{
			return X == otherSlicePosition.X || Y == otherSlicePosition.Y;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SlicePosition))
				return false;

			var otherSlicePosition = (SlicePosition)obj;

			return otherSlicePosition.X == X && otherSlicePosition.Y == Y;
		}

		public bool Equals(SlicePosition otherSlicePosition)
		{
			return otherSlicePosition.X == X && otherSlicePosition.Y == Y;
		}

		public static bool operator ==(SlicePosition posA, SlicePosition posB)
		{
			return posA.Equals(posB);
		}

		public static bool operator !=(SlicePosition posA, SlicePosition posB)
		{
			return !posA.Equals(posB);
		}
	}
#pragma warning restore 661, 659
}
