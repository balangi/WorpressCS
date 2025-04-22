using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Blocks
{
    public class BlockList : IEnumerable<Block>, ICollection<Block>
    {
        /// <summary>
        /// Original list of parsed block data, or block instances.
        /// </summary>
        private readonly List<Block> _blocks = new List<Block>();

        /// <summary>
        /// All available context of the current hierarchy.
        /// </summary>
        private readonly Dictionary<string, object> _availableContext = new Dictionary<string, object>();

        /// <summary>
        /// Block type registry to use in constructing block instances.
        /// </summary>
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="blocks">Array of parsed block data, or block instances.</param>
        /// <param name="availableContext">Optional array of ancestry context values.</param>
        /// <param name="dbContext">Optional block type registry.</param>
        public BlockList(IEnumerable<Block> blocks, Dictionary<string, object> availableContext = null, AppDbContext dbContext = null)
        {
            _blocks.AddRange(blocks);
            _availableContext = availableContext ?? new Dictionary<string, object>();
            _dbContext = dbContext ?? new AppDbContext();
        }

        /// <summary>
        /// Returns true if a block exists by the specified block index, or false otherwise.
        /// </summary>
        public bool OffsetExists(int index)
        {
            return index >= 0 && index < _blocks.Count;
        }

        /// <summary>
        /// Returns the value by the specified block index.
        /// </summary>
        public Block OffsetGet(int index)
        {
            if (OffsetExists(index))
            {
                return _blocks[index];
            }

            throw new IndexOutOfRangeException($"Block at index {index} does not exist.");
        }

        /// <summary>
        /// Assigns a block value by the specified block index.
        /// </summary>
        public void OffsetSet(int index, Block value)
        {
            if (index == -1)
            {
                _blocks.Add(value);
            }
            else if (OffsetExists(index))
            {
                _blocks[index] = value;
            }
            else
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range.");
            }
        }

        /// <summary>
        /// Removes a block by the specified block index.
        /// </summary>
        public void OffsetUnset(int index)
        {
            if (OffsetExists(index))
            {
                _blocks.RemoveAt(index);
            }
            else
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range.");
            }
        }

        /// <summary>
        /// Rewinds back to the first element of the Iterator.
        /// </summary>
        public IEnumerator<Block> GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        /// <summary>
        /// Returns the count of blocks in the list.
        /// </summary>
        public int Count => _blocks.Count;

        /// <summary>
        /// Adds a block to the list.
        /// </summary>
        public void Add(Block block)
        {
            _blocks.Add(block);
        }

        /// <summary>
        /// Clears all blocks from the list.
        /// </summary>
        public void Clear()
        {
            _blocks.Clear();
        }

        /// <summary>
        /// Checks if a block exists in the list.
        /// </summary>
        public bool Contains(Block block)
        {
            return _blocks.Contains(block);
        }

        /// <summary>
        /// Copies the blocks to an array.
        /// </summary>
        public void CopyTo(Block[] array, int arrayIndex)
        {
            _blocks.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes a block from the list.
        /// </summary>
        public bool Remove(Block block)
        {
            return _blocks.Remove(block);
        }

        /// <summary>
        /// Returns true if the collection is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Non-generic enumerator for compatibility.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}