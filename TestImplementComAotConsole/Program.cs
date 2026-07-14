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
		IWICImagingFactory2* factory2 = null;

		try
		{
			// 準備
			PInvoke.CoCreateInstance(PInvoke.CLSID_WICImagingFactory2, null, CLSCTX.CLSCTX_INPROC_SERVER, out factory2).ThrowOnFailure();

			// 自作 COM と HGlobal の両方に同じ内容を書き込む
			Byte[] custom = WriteToCustomStream(factory2);
			Byte[] hglobal = WriteToHGlobal(factory2);

			// 本当に内容が同じか確認
			Console.WriteLine("◆内容の同一性チェック");
			if (!custom.AsSpan().SequenceEqual(hglobal))
			{
				throw new Exception("内容が異なっています。");
			}
			Console.WriteLine("同一です。");
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
	/// 画像を書き込むコア部分
	/// </summary>
	private static unsafe Byte[] WriteCore(String id, IWICImagingFactory2* factory2, IStream* stream)
	{
		IWICBitmapEncoder* encoder = null;
		IWICBitmapFrameEncode* frame = null;
		IPropertyBag2* bag = null;

		try
		{
			Console.WriteLine($"◆{id} への書き込み");

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

			// 書き込んだ内容を読み出す
			Byte[] data = new Byte[stat.cbSize];
			stream->Seek(0, SeekOrigin.Begin).ThrowOnFailure();
			stream->Read(data).ThrowOnFailure();

			// ファイルに出力
			String fileName = id + ".png";
			File.WriteAllBytes(fileName, data);
			Console.WriteLine($"{fileName} への出力完了");

			return data;
		}
		finally
		{
			// 後片付け
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
		}
	}

	/// <summary>
	/// 画像を自作 COM に書き込む
	/// </summary>
	/// <param name="factory2"></param>
	private static unsafe Byte[] WriteToCustomStream(IWICImagingFactory2* factory2)
	{
		IStream* stream = null;

		try
		{
			StrategyBasedComWrappers cw = new();
			Com.CustomStream customStream = new();
			IUnknown* unk = (IUnknown*)cw.GetOrCreateComInterfaceForObject(customStream, CreateComInterfaceFlags.None);
			unk->QueryInterface(out stream).ThrowOnFailure();
			unk->Release();
			return WriteCore("CustomCOM", factory2, stream);
		}
		finally
		{
			if (stream != null)
			{
				stream->Release();
			}
		}
	}

	/// <summary>
	/// 画像を ToHGlobal に書き込む
	/// </summary>
	private static unsafe Byte[] WriteToHGlobal(IWICImagingFactory2* factory2)
	{
		IStream* stream = null;

		try
		{
			PInvoke.CreateStreamOnHGlobal(HGLOBAL.Null, true, &stream).ThrowOnFailure();
			return WriteCore("HGlobal", factory2, stream);
		}
		finally
		{
			if (stream != null)
			{
				stream->Release();
			}
		}
	}
}
