using System;
using System.Collections.Generic;
using System.Text;
using DebugStuff;
using UnityEngine;

namespace Puzzle
{
	public class PuzzleField
	{
		private const int MinPuzzleSize = 2;

		private int[,] _slices;
		private SlicePosition _emptyCellPosition;


		//=== Props ===========================================================

		public int Size { get; private set; }

		public SlicePosition EmptyCellPosition
		{
			get { return _emptyCellPosition; }

			private set
			{
				_emptyCellPosition = value;
				_slices[_emptyCellPosition.X, _emptyCellPosition.Y] = 0;
			}
		}


		//=== Public ==========================================================

		public void InitSize(int size)
		{
			if (size < MinPuzzleSize)
				throw new Exception(string.Format("Wrong size: {0}! Must be greater than {1}", size, MinPuzzleSize));

			Size = size;
			ResetSlicesSize();
		}

		public bool TryToSlide(SlicePosition touchPosition, out List<SliceMove> moves)
		{
			moves = null;
			if (touchPosition == EmptyCellPosition || !touchPosition.OnSameLine(EmptyCellPosition))
				return false;

			moves = new List<SliceMove>();
			if (touchPosition.X == EmptyCellPosition.X)
			{
				int constX = EmptyCellPosition.X;
				int toEmptyCellIncr = touchPosition.Y > EmptyCellPosition.Y ? -1 : 1;
				for (int i = EmptyCellPosition.Y; i != touchPosition.Y; i -= toEmptyCellIncr)
				{
					_slices[constX, i] = _slices[constX, i - toEmptyCellIncr];
					moves.Add(new SliceMove(_slices[constX, i], new SlicePosition(constX, i)));
				}
				EmptyCellPosition = new SlicePosition(constX, touchPosition.Y);
			}
			else
			{
				int constY = EmptyCellPosition.Y;
				int toEmptyCellIncr = touchPosition.X > EmptyCellPosition.X ? -1 : 1;
				for (int i = EmptyCellPosition.X; i != touchPosition.X; i -= toEmptyCellIncr)
				{
					_slices[i, constY] = _slices[i - toEmptyCellIncr, constY];
					moves.Add(new SliceMove(_slices[i, constY], new SlicePosition(i, constY)));
				}
				EmptyCellPosition = new SlicePosition(touchPosition.X, constY);
			}
			return true;
		}

		public string SlicesToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int y = 0; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					var sliceIndex = _slices[x, y];
					if (sliceIndex == 0)
						sb.Append("__\t");
					else
						sb.AppendFormat("{0:00}\t", sliceIndex);
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}


		//=== Private =========================================================

		/// <summary>
		/// Создает заново число и первоначальные позиции плашек: слева направо, сверху вниз, последняя (правая нижняя) позиция пуста.
		/// Возвращает, были ли изменения размера поля
		/// </summary>
		private bool ResetSlicesSize()
		{
			//Левый верхний угол [0,0]
			bool hasChangedSize = false;
			if (_slices == null || _slices.Length != Size * Size)
			{
				hasChangedSize = true;
				_slices = new int[Size, Size];
			}

			int sliceNum = 1;
			for (int vertIdx = 0; vertIdx < Size; vertIdx++)
			{
				for (int horizIdx = 0; horizIdx < Size; horizIdx++)
				{
					_slices[horizIdx, vertIdx] = sliceNum++;
				}
			}
			_slices[Size - 1, Size - 1] = 0;
			EmptyCellPosition = new SlicePosition(Size - 1, Size - 1);

			return hasChangedSize;
		}
	}
}
