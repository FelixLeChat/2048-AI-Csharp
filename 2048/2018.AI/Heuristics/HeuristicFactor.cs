namespace _2048.AI.Heuristics
{
    /// <summary>
    /// The lower is the evaluation of heuristic the better!!!
    /// </summary>
    public class HeuristicFactor 
    {
        public float LostPenalty { get; set; }
        public float MonoticityPower { get; set; }
        public float MonoticityWeight { get; set; }
        public float SumPower { get; set; }
        public float SumWeight { get; set; }
        public float MergeWeigth { get; set; }
        public float EmptyWeigth { get; set; }
        public float FillWeigth { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as HeuristicFactor;
            if (item != null)
            {
                return item.LostPenalty == LostPenalty
                       && item.MonoticityPower == MonoticityPower
                       && item.MonoticityWeight == MonoticityWeight
                       && item.SumPower == SumPower
                       && item.SumWeight == SumWeight
                       && item.MergeWeigth == MergeWeigth
                       && item.EmptyWeigth == EmptyWeigth
                       && item.FillWeigth == FillWeigth;
            }

            return false;
        }


        public static HeuristicFactor GetSomeHeuristic()
        {
            // Best factor according to the github
            return new HeuristicFactor()
            {
                LostPenalty = 200000.0f,
                MonoticityPower = 4.0f,
                MonoticityWeight = -47.0f,
                SumPower = 3.5f,
                SumWeight = -11.0f,
                MergeWeigth = 700.0f,
                EmptyWeigth = 270.0f,
                FillWeigth = 270.0f
            };

            // Try maximize the number of empty tiles
            //return new HeuristicFactor()
            //{
            //    LostPenalty = 0.0f,
            //    MonoticityPower = 4.0f,
            //    MonoticityWeight = -60.0f,
            //    SumPower = 0.0f,
            //    SumWeight = 0.0f,
            //    MergeWeigth = 0.0f,
            //    EmptyWeigth = 0.0f,
            //    FillWeigth = 0.0f
            //};
        }
    }
}