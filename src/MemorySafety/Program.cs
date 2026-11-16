using System.Runtime.InteropServices;

// A ticket barcode: 4-byte magic "DDC!", then a 4-byte little-endian ticket number.
byte[] barcode = [(byte)'D', (byte)'D', (byte)'C', (byte)'!', 0x2A, 0x00, 0x00, 0x00];

// 1. Taking an address / fixed / sizeof no longer need an unsafe context —
//    creating a pointer is harmless, *dereferencing* it is where the danger is.
int answer = 42;
int* pointer = &answer;
Console.WriteLine($"sizeof(long) = {sizeof(long)}, pointer created without 'unsafe'");

// 2. Dereferencing still requires an unsafe block:
// Console.WriteLine(*pointer);          // ← error under the new rules
unsafe { Console.WriteLine($"*pointer = {*pointer}"); }

// 3. 'unsafe' on a signature now means "requires unsafe": callers must opt in explicitly.
fixed (byte* start = barcode)
{
    // int number = Barcode.ReadInt32(start + 4);   // ← error: caller must be in an unsafe context
    int number = unsafe(Barcode.ReadInt32(start + 4));   // unsafe *expression* (new)
    Console.WriteLine($"Ticket number: {number}");
}

// 4. Safe wrapper: the unsafety stays inside, callers don't need to care.
Console.WriteLine($"Valid barcode? {Barcode.IsValid(barcode)}");

// 5. extern members must now state whether they are safe or unsafe to call.
Console.WriteLine($"Process id via libc: {Native.getpid()}");

static class Barcode
{
    public static unsafe int ReadInt32(byte* source)
    {
        unsafe { return *(int*)source; }
    }

    public static bool IsValid(ReadOnlySpan<byte> data) =>
        data.Length == 8 && data[..4].SequenceEqual("DDC!"u8);
}

static partial class Native
{
    // 'safe': no pointers, no way to corrupt memory — callers need no unsafe context.
    [LibraryImport("libc")]
    internal static safe partial int getpid();

    // 'unsafe': takes a raw pointer — callers must be in an unsafe context.
    [LibraryImport("libc")]
    internal static unsafe partial nint strlen(byte* str);
}
