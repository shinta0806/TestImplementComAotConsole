using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

using Windows.Win32.Foundation;

using Windows.Win32.System.Com;

namespace TestImplementComAotConsole.Com;

[GeneratedComClass]
public partial class CustomStream : IStream
{
	// --------------------------------------------------------------------
	// ISequentialStream
	// --------------------------------------------------------------------

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public unsafe HRESULT Read(void* pv, UInt32 cb, [Optional] UInt32* pcbRead)
	{
		Console.WriteLine("Read()");
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public unsafe HRESULT Write(void* pv, UInt32 cb, [Optional] UInt32* pcbWritten)
	{
		Console.WriteLine("Write()");
		return HRESULT.S_OK;
	}

	// --------------------------------------------------------------------
	// IStream
	// --------------------------------------------------------------------

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public unsafe HRESULT Seek(Int64 dlibMove, SeekOrigin dwOrigin, [Optional] UInt64* plibNewPosition)
	{
		Console.WriteLine("Seek()");
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
		return HRESULT.S_OK;
	}

	[PreserveSig()]
	[return: MarshalAs(UnmanagedType.Error)]
	public HRESULT Clone(out IStream ppstm)
	{
		ppstm = this;
		return HRESULT.S_OK;
	}
}
