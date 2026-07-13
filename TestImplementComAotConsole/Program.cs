using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Imaging.D2D;
using Windows.Win32.System.Com;

namespace TestImplementComAotConsole;

internal class Program
{
	// ====================================================================
	// メイン
	// ====================================================================

	public static unsafe void Main(String[] args)
	{
		IWICImagingFactory2* factory2 = null;

		try
		{
			// 準備
			PInvoke.CoInitialize();
			PInvoke.CoCreateInstance(PInvoke.CLSID_WICImagingFactory2, null, CLSCTX.CLSCTX_INPROC_SERVER, out factory2).ThrowOnFailure();
			String inputPath = GetInputPath(args);
			Console.WriteLine("Hello, World!");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"エラー：{ex.Message}");
		}
		finally
		{
			if (factory2 != null)
			{
				factory2->Release();
			}
			PInvoke.CoUninitialize();
		}
	}

	// ====================================================================
	// private 関数
	// ====================================================================

	/// <summary>
	/// 引数から画像ファイルパスを取得
	/// </summary>
	/// <param name="args"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	private static String GetInputPath(String[] args)
	{
		if (args.Length == 0)
		{
			throw new Exception("画像ファイルのパスを引数で指定してください。");
		}
		return args[0];
	}

}
