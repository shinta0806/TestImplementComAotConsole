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
		IWICImagingFactory2* factory2 = null;
		IStream* stream = null;
		IWICBitmapEncoder* encoder = null;
		IWICBitmapFrameEncode* frame = null;
		IPropertyBag2* bag = null;

		try
		{
			// 準備
			PInvoke.CreateStreamOnHGlobal(HGLOBAL.Null, true, &stream).ThrowOnFailure();
			PInvoke.CoCreateInstance(PInvoke.CLSID_WICImagingFactory2, null, CLSCTX.CLSCTX_INPROC_SERVER, out factory2).ThrowOnFailure();

			// PNG エンコーダー
			factory2->CreateEncoder(PInvoke.GUID_ContainerFormatPng, Guid.Empty, &encoder).ThrowOnFailure();
			encoder->Initialize(stream, WICBitmapEncoderCacheOption.WICBitmapEncoderNoCache).ThrowOnFailure();

			// 2x2 px フレーム作成
			encoder->CreateNewFrame(&frame, &bag).ThrowOnFailure();
			frame->Initialize(bag);
			frame->SetSize(2, 2);
			Guid format = PInvoke.GUID_WICPixelFormat32bppBGRA;
			frame->SetPixelFormat(ref format).ThrowOnFailure();

			// BGRA 書き込み
			Byte[] pixels = [
				255, 0, 0, 255,
				0, 255, 0, 255,
				0, 0, 255, 255,
				255, 255, 255, 255,
			];
			frame->WritePixels(2, 2 * 4, pixels).ThrowOnFailure();
			frame->Commit();
			encoder->Commit();

			// 書き込みサイズ確認
			stream->Stat(out STATSTG stat, (UInt32)STATFLAG.STATFLAG_NONAME);
			Console.WriteLine($"書き込み完了：{stat.cbSize} バイト");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"エラー：{ex.Message}");
		}
		finally
		{
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
			PInvoke.CoUninitialize();
		}
	}

	// ====================================================================
	// private 関数
	// ====================================================================


}
