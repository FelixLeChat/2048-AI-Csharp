using System;
using _2048.AI.Helper;
using _2048.AI.Heuristics;
using _2048.AI.Model;
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


    public class OptimizeScorer : IOptimizedScore, IScore
    {
        private static HeuristicFactor _factor;
        static readonly float[] HeuristicTable = new float[65536];

        public static float EvaluateHeuristic(Board board)
        {
            return HeuristicTable[(board >> 0) & BitArrayHelper.ROW_MASK] +
                   HeuristicTable[(board >> 16) & BitArrayHelper.ROW_MASK] +
                   HeuristicTable[(board >> 32) & BitArrayHelper.ROW_MASK] +
                   HeuristicTable[(board >> 48) & BitArrayHelper.ROW_MASK];
        }

        public static void InitLookupTable(HeuristicFactor factor)
        {
            if(factor != null && factor.Equals(_factor))
            {
                return;
            }

            _factor = factor;

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
            HeuristicTable[row] = _factor.LostPenalty +
                                   _factor.EmptyWeigth * info.EmptyCount +
                                   _factor.MergeWeigth * info.MergableCount +
                                   _factor.MonoticityWeight * info.Monoticity +
                                   _factor.SumWeight * info.Sum + 
                                   _factor.FillWeigth * info.FillCount;
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
                info.Sum += (float) Math.Pow(rank, _factor.SumPower);
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

            for (var i = 1; i < 4; ++i)
            {
                if (line[i - 1] > line[i])
                {
                    monoticityLeft += Math.Pow(line[i - 1], _factor.MonoticityPower) -
                                      Math.Pow(line[i], _factor.MonoticityPower);
                }
                else
                {
                    monoticityRight += Math.Pow(line[i], _factor.MonoticityPower) -
                                     Math.Pow(line[i - 1], _factor.MonoticityPower);
                }
            }

            return (float)Math.Min(monoticityLeft, monoticityRight);
        }

        public double GetScore(IBoard board)
        {
            var optRepresentation = board.GetBitArrayRepresentation();
            return EvaluateHeuristic(optRepresentation);
        }

        public double Score(TreeNode node)
        {
            return GetScore(node.Board);
        }
    }
}