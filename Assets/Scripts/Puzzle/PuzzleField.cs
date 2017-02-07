using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleField
{
	private const int MinPuzzleSize = 2;

	private int[,] _slices;

	public int Size { get; private set; }

	public void InitSize(int size)
	{
		if (size < MinPuzzleSize)
			throw new Exception(string.Format("Wrong size: {0}! Must be greater than {1}", size, MinPuzzleSize));

		Size = size;
		ResetSlicesSize();
	}

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
			_slices = new int[Size - 1, Size - 1];
		}

		int sliceNum = 1;
		for (int horizIdx = 0; horizIdx < Size; horizIdx++)
		{
			for (int vertIdx = 0; vertIdx < Size; vertIdx++)
			{
				_slices[horizIdx, vertIdx] = sliceNum++;
			}
		}
		_slices[Size, Size] = 0;
		return hasChangedSize;
	}
}
