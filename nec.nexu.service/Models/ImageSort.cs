using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ImageSort
{

    public class KeyDistributor<TKey>
    {
        private int _numBuckets;
        private int _pageSize;
        private SHA1 _sha;

        public int CalculateBucketIndex(TKey key)
        {
            if (_numBuckets == 1)
            {
                return 0;
            }

            // Create a SHA1 hash of the provided key
            byte[] result;
            byte[] data = BitConverter.GetBytes(key.GetHashCode());
            result = _sha.ComputeHash(data);

            // Split the hash into 4 integer numbers
            uint n1 = BitConverter.ToUInt32(result, 0);
            uint n2 = BitConverter.ToUInt32(result, 4);
            uint n3 = BitConverter.ToUInt32(result, 8);
            uint n4 = BitConverter.ToUInt32(result, 12);
            uint n5 = BitConverter.ToUInt32(result, 16);

            // Apply XOR to derive a combined number
            uint index = n1 ^ n2 ^ n3 ^ n4 ^ n5;

            // Calculate the bucket index
            int bucketIndex = Convert.ToInt32(Math.Floor((double)(index / _pageSize)));

            return bucketIndex;
        }

        public KeyDistributor(int numBuckets)
        {
            _numBuckets = numBuckets;

            if (_numBuckets > 1)
            {
                // Based on the number of buckets calculate the page size.
                // It will be to link a given key to a specific bucket
                _pageSize = Convert.ToInt32(Math.Ceiling((double)(uint.MaxValue / numBuckets)));
                _sha = new SHA1Managed();
            }
        }
    }
}
