using System;
using System.Collections.Generic;

public class MinHeap<T> where T : IComparable<T>
{

	private T[] minHeap;
	private Dictionary<T, int> indices;

	public MinHeap()
	{
		minHeap = new T[64];
		indices = new Dictionary<T, int>();
		size = 0;
	}

	public int size
	{
		get; private set;
	}

	public void Add(T item)
	{
		if (minHeap.Length == size)
		{
			Expand();
		}

		minHeap[size++] = item;
		WalkUp(size - 1);
	}

	public T Peek()
	{
		return minHeap[0];
	}

	public T Pop()
	{
		T ret = minHeap[0];
		indices.Remove(ret);
		minHeap[0] = minHeap[--size];
		WalkDown(0);
		return ret;
	}

	public bool Contains(T item)
	{
		int index;
		return indices.TryGetValue(item, out index) &&
			index >= 0 && index < minHeap.Length;
	}

	public void PriorityLowered(T item)
	{
		int index;
		if (indices.TryGetValue(item, out index) &&
			index >= 0 && index < minHeap.Length)
		{
			WalkUp(index);
		}
	}

	private void WalkDown(int startingIndex)
	{
		int index = startingIndex;
		T walkingItem = minHeap[index];
		while (index < size)
		{
			int leftChild = GetLeftChildIndex(index);
			int rightChild = GetRightChildIndex(index);
			if (leftChild >= size)
			{
				break;
			}
			int child = leftChild;
			if (rightChild < size &&
				minHeap[rightChild].CompareTo(minHeap[leftChild]) < 0)
			{
				child = rightChild;
			}

			if (minHeap[child].CompareTo(walkingItem) < 0)
			{
				minHeap[index] = minHeap[child];
				indices[minHeap[index]] = index;
				index = child;
			}
			else
			{
				break;
			}
		}

		minHeap[index] = walkingItem;
		indices[minHeap[index]] = index;
	}

	private void WalkUp(int startingIndex)
	{
		int index = startingIndex;
		T walkingItem = minHeap[index];
		while (index > 0)
		{
			int parent = GetParentIndex(index);
			if (walkingItem.CompareTo(minHeap[parent]) < 0)
			{
				minHeap[index] = minHeap[parent];
				indices[minHeap[index]] = index;
				index = parent;
			}
			else
			{
				break;
			}
		}

		minHeap[index] = walkingItem;
		indices[minHeap[index]] = index;
	}

	private static int GetLeftChildIndex(int parentIndex)
	{
		return parentIndex * 2 + 1;
	}

	private static int GetRightChildIndex(int parentIndex)
	{
		return parentIndex * 2 + 2;
	}

	private static int GetParentIndex(int childIndex)
	{
		return (childIndex - 1) / 2;
	}

	private void Expand()
	{
		T[] newMinHeap = new T[minHeap.Length * 2];
		minHeap.CopyTo(newMinHeap, 0);
		minHeap = newMinHeap;
	}
}
