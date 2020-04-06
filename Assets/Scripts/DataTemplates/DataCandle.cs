// output format from https://finnhub.io/docs/api#stock-candles
public class DataCandle
{
    /// <summary>
    /// close prices
    /// </summary>
    public float[] c { get; set; }
    /// <summary>
    /// high prices
    /// </summary>
    public float[] h { get; set; }
    /// <summary>
    /// low prices
    /// </summary>
    public float[] l { get; set; }
    /// <summary>
    /// open prices
    /// </summary>
    public float[] o { get; set; }
    /// <summary>
    /// timestamps
    /// </summary>
    public float[] t { get; set; }
    /// <summary>
    /// volume data
    /// </summary>
    public float[] v { get; set; }
    /// <summary>
    /// response status. "ok" or "no_data".
    /// </summary>
    public string s { get; set; }
}