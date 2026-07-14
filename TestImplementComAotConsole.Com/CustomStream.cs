// ============================================================================
// 
// IStream インターフェースを実装する自作 COM
// 
// ============================================================================

// ----------------------------------------------------------------------------
// サンプルプログラムが稼働する最小限の内容
// ----------------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

using Windows.Win32.Foundation;

using Windows.Win32.System.Com;

namespace TestImplementComAotConsole.Com;

[GeneratedComClass]
public partial class CustomStream : IStream
{
	// ====================================================================
	// インターフェース実装
	// ====================================================================

	// --------------------------------------------------------------------
	// ISequentialStream
	// --------------------------------------------------------------------

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public unsafe HRESULT Read(void* pv, UInt32 cb, [Optional] UInt32* pcbRead)
	{
		Span<Byte> dest = new(pv, (Int32)cb);
		_contents.CopyTo(dest);
		if (pcbRead != null)
		{
			*pcbRead = cb;
		}
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public unsafe HRESULT Write(void* pv, UInt32 cb, [Optional] UInt32* pcbWritten)
	{
		_contents = new Byte[cb];
		Span<Byte> src = new(pv, (Int32)cb);
		src.CopyTo(_contents);
		if (pcbWritten != null)
		{
			*pcbWritten = cb;
		}
		return HRESULT.S_OK;
	}

	// --------------------------------------------------------------------
	// IStream
	// --------------------------------------------------------------------

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public unsafe HRESULT Seek(Int64 dlibMove, SeekOrigin dwOrigin, [Optional] UInt64* plibNewPosition)
	{
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public HRESULT SetSize(UInt64 libNewSize)
	{
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public unsafe HRESULT CopyTo(IStream pstm, UInt64 cb, [Optional] UInt64* pcbRead, [Optional] UInt64* pcbWritten)
	{
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public HRESULT Commit(UInt32 grfCommitFlags)
	{
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public HRESULT Revert()
	{
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public HRESULT LockRegion(UInt64 libOffset, UInt64 cb, LOCKTYPE dwLockType)
	{
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public HRESULT UnlockRegion(UInt64 libOffset, UInt64 cb, UInt32 dwLockType)
	{
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public unsafe HRESULT Stat(STATSTG* pstatstg, STATFLAG grfStatFlag)
	{
		pstatstg->cbSize = (UInt64)_contents.Length;
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public HRESULT Clone(out IStream ppstm)
	{
		ppstm = this;
		return HRESULT.S_OK;
	}

	// ====================================================================
	// private 変数
	// ====================================================================

	/// <summary>
	/// 書き込まれた内容
	/// </summary>
	private Byte[] _contents = Array.Empty<Byte>();
}
