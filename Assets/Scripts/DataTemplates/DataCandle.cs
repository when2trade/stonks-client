// output format from https://finnhub.io/docs/api#stock-candles
public class DataCandle
{
    public float[] c { get; set; }
    public float[] h { get; set; }
    public float[] l { get; set; }
    public float[] o { get; set; }
    public float[] t { get; set; }
    public float[] v { get; set; }
    public string s { get; set; }
}