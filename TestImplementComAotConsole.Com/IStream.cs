// ============================================================================
// 
// IStream インターフェース（COM 実装時用）
// 
// ============================================================================

// ----------------------------------------------------------------------------
// allowMarshaling を true にした CsWin32 が生成したものがベース
// ----------------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

using Windows.Win32.Foundation;

using Windows.Win32.System.Com;

namespace TestImplementComAotConsole.Com;

[GeneratedComInterface]
[Guid("0000000C-0000-0000-C000-000000000046")]
public partial interface IStream
{
	// --------------------------------------------------------------------
	// ISequentialStream
	// --------------------------------------------------------------------

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	unsafe HRESULT Read(void* pv, UInt32 cb, [Optional] UInt32* pcbRead);

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	unsafe HRESULT Write(void* pv, UInt32 cb, [Optional] UInt32* pcbWritten);

	// --------------------------------------------------------------------
	// IStream
	// --------------------------------------------------------------------

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	unsafe HRESULT Seek(Int64 dlibMove, SeekOrigin dwOrigin, [Optional] UInt64* plibNewPosition);

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	HRESULT SetSize(UInt64 libNewSize);

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	unsafe HRESULT CopyTo(IStream pstm, UInt64 cb, [Optional] UInt64* pcbRead, [Optional] UInt64* pcbWritten);

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	HRESULT Commit(UInt32 grfCommitFlags);

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	HRESULT Revert();

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	HRESULT LockRegion(UInt64 libOffset, UInt64 cb, LOCKTYPE dwLockType);

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	HRESULT UnlockRegion(UInt64 libOffset, UInt64 cb, UInt32 dwLockType);

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	unsafe HRESULT Stat(STATSTG* pstatstg, STATFLAG grfStatFlag);

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	HRESULT Clone(out IStream ppstm);
}
