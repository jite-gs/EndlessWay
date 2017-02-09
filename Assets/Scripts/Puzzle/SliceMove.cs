namespace Puzzle
{
	public struct SliceMove
	{
		public int SliceIndex;
		public SlicePosition NewSlicePosition;

		public SliceMove(int sliceIndex, SlicePosition newSlicePosition)
		{
			SliceIndex = sliceIndex;
			NewSlicePosition = newSlicePosition;
		}

		public override string ToString()
		{
			return string.Format("[SliceMove[{0}]: {1}]", SliceIndex, NewSlicePosition);
		}
	}
}
