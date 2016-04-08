using System;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using _2048.AI.Helper;
using _2048.AI.Heuristics;
using _2048.AI.Model.Core;


using Board = System.UInt64;
using Row = System.UInt16;

namespace _2048.AI.Scoring
{
    public struct RowInfo
    {
        public float Sum;
        public int EmptyCount;
        public int FillCount;
        public int MergableCount;
        public float Monoticity;
    }


    public class OptimizeScorer 
    {
        private static HeuristicFactor Factor;
        static readonly float[] _heuristicTable = new float[65536];

        public static float EvaluateHeuristic(Board board)
        {
            return _heuristicTable[(board >> 0) & BitArrayHelper.ROW_MASK] +
                   _heuristicTable[(board >> 16) & BitArrayHelper.ROW_MASK] +
                   _heuristicTable[(board >> 32) & BitArrayHelper.ROW_MASK] +
                   _heuristicTable[(board >> 48) & BitArrayHelper.ROW_MASK];
        }

        public static void InitLookupTable(HeuristicFactor factor)
        {
            if(factor != null && factor.Equals(Factor))
            {
                return;
            }

            Factor = factor;

            for (int row = 0; row < 65536; ++row)
            {
                // Transform the uint of row into [nible1, nible2, nible3, nible 4]
                int[] line = new int[]
                {
                    (row >>  0) & 0xf,
                    (row >>  4) & 0xf,
                    (row >>  8) & 0xf,
                    (row >> 12) & 0xf
                };

                CalculateRowScore(row, line);
            }
        }

        //// Calculate the score of this row
        private static void CalculateRowScore(int row, int[] line)
        {
            var info = GetRowInfo(line);
            _heuristicTable[row] = Factor.LostPenalty +
                                   Factor.EmptyWeigth * info.EmptyCount +
                                   Factor.MergeWeigth * info.MergableCount +
                                   Factor.MonoticityWeight * info.Monoticity +
                                   Factor.SumWeight * info.Sum + 
                                   Factor.FillWeigth * info.FillCount;
        }

        private static RowInfo GetRowInfo(int[] line)
        {
            RowInfo info;
            info.EmptyCount = 0;
            info.MergableCount = 0;
            info.Sum = 0;
            info.Monoticity = GetMonotinity(line);

            int prev = 0;
            int counter = 0;
            for (int i = 0; i < 4; ++i)
            {
                int rank = line[i];
                info.Sum += (float) Math.Pow(rank, Factor.SumPower);
                if (rank == 0)
                {
                    info.EmptyCount ++;
                }
                else
                {
                    if (prev == rank)
                    {
                        counter ++;
                    }
                    else if (counter > 0)
                    {
                        info.MergableCount += 1 + counter;
                        counter = 0;
                    }
                    prev = rank;
                }
            }

            if (counter > 0)
            {
                info.MergableCount += 1 + counter;
            }

            info.FillCount = 16 - info.EmptyCount;

            return info;
        }


        private static float GetMonotinity(int[] line)
        {
            double monoticityLeft = 0;
            double monoticityRight = 0;

            for (int i = 1; i < 4; ++i)
            {
                if (line[i - 1] > line[i])
                {
                    monoticityLeft += Math.Pow(line[i - 1], Factor.MonoticityPower) -
                                      Math.Pow(line[i], Factor.MonoticityPower);
                }
                else
                {
                    monoticityRight += Math.Pow(line[i], Factor.MonoticityPower) -
                                     Math.Pow(line[i - 1], Factor.MonoticityPower);
                }
            }

            return (float)Math.Min(monoticityLeft, monoticityRight);
        }
    }
}