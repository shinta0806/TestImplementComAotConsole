// ============================================================================
// 
// IStream を用いて PNG 画像を書き込む
// 
// ============================================================================

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Imaging;
using Windows.Win32.Graphics.Imaging.D2D;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;

namespace TestImplementComAotConsole;

internal class Program
{
	// ====================================================================
	// メイン
	// ====================================================================

	public static unsafe void Main(String[] args)
	{
		try
		{
			// 準備
			PInvoke.CoCreateInstance(PInvoke.CLSID_WICImagingFactory2, null, CLSCTX.CLSCTX_INPROC_SERVER, out IWICImagingFactory2* factory2).ThrowOnFailure();

			//WriteToHGlobal(factory2);
			WriteToCustomStream(factory2);


		}
		catch (Exception ex)
		{
			Console.WriteLine($"エラー：{ex.Message}");
		}
		finally
		{
			PInvoke.CoUninitialize();
		}
	}

	// ====================================================================
	// private 関数
	// ====================================================================

	/// <summary>
	/// 画像を書き込むコア部分
	/// </summary>
	private static unsafe void WriteCore(IWICImagingFactory2* factory2, IStream* stream)
	{
		IWICBitmapEncoder* encoder = null;
		IWICBitmapFrameEncode* frame = null;
		IPropertyBag2* bag = null;

		try
		{
			// PNG エンコーダー
			factory2->CreateEncoder(PInvoke.GUID_ContainerFormatPng, Guid.Empty, &encoder).ThrowOnFailure();
			encoder->Initialize(stream, WICBitmapEncoderCacheOption.WICBitmapEncoderNoCache).ThrowOnFailure();

			// 2x2 px フレーム作成
			encoder->CreateNewFrame(&frame, &bag).ThrowOnFailure();
			frame->Initialize(bag).ThrowOnFailure();
			frame->SetSize(2, 2).ThrowOnFailure();
			Guid format = PInvoke.GUID_WICPixelFormat32bppBGRA;
			frame->SetPixelFormat(ref format).ThrowOnFailure().ThrowOnFailure();

			// BGRA 書き込み
			Byte[] pixels = [
				255, 0, 0, 255,
				0, 255, 0, 255,
				0, 0, 255, 255,
				255, 255, 255, 255,
			];
			frame->WritePixels(2, 2 * 4, pixels).ThrowOnFailure();
			frame->Commit().ThrowOnFailure();
			encoder->Commit().ThrowOnFailure();

			// 書き込みサイズ確認
			stream->Stat(out STATSTG stat, (UInt32)STATFLAG.STATFLAG_NONAME).ThrowOnFailure();
			Console.WriteLine($"IStream への書き込み完了：{stat.cbSize} バイト");

			// ファイルに書き出す
			Byte[] data = new Byte[stat.cbSize];
			stream->Seek(0, SeekOrigin.Begin).ThrowOnFailure();
			stream->Read(data).ThrowOnFailure();
			File.WriteAllBytes("Out.png", data);
			Console.WriteLine("ファイルへの書き込み完了");
		}
		finally
		{
			// 引数で受け取った物も含めて後片付け
			if (bag != null)
			{
				bag->Release();
			}
			if (frame != null)
			{
				frame->Release();
			}
			if (encoder != null)
			{
				encoder->Release();
			}
			if (stream != null)
			{
				stream->Release();
			}
			if (factory2 != null)
			{
				factory2->Release();
			}
		}
	}

	/// <summary>
	/// 画像を自作 COM で書き込む
	/// </summary>
	/// <param name="factory2"></param>
	private static unsafe void WriteToCustomStream(IWICImagingFactory2* factory2)
	{
		StrategyBasedComWrappers cw = new();
		Com.CustomStream customStream = new();
		IUnknown* unk = (IUnknown*)cw.GetOrCreateComInterfaceForObject(customStream, CreateComInterfaceFlags.None);
		unk->QueryInterface(out IStream* stream).ThrowOnFailure();
		WriteCore(factory2, stream);
	}

	/// <summary>
	/// 画像をメモリに書き込む
	/// </summary>
	private static unsafe void WriteToHGlobal(IWICImagingFactory2* factory2)
	{
		IStream* stream = null;
		PInvoke.CreateStreamOnHGlobal(HGLOBAL.Null, true, &stream).ThrowOnFailure();
		WriteCore(factory2, stream);
	}


}
