namespace ConsoleApp1.Settings;

/// <summary>
/// 設定項目のサンプルクラス
/// </summary>
public class SampleSetting
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Description { get; set; }
    /// <summary>
    /// Environment variable value
    /// 
    /// key in .env file: SampleSettings__EnvValue=abc123
    /// in prompt:  $env:SampleSettings:EnvValue="xyz987"
    /// 
    /// プロンプトでの環境変数の設定が優先される
    /// </summary>
    public string? EnvValue { get; set; }
}