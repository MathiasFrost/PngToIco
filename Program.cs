using System.Drawing;

if (args.Length == 0)
{
    Console.WriteLine("No file path provided.");
    Console.WriteLine("Usage: pngtoico path/to/png.png");
    return;
}

string filePath = args[0];

// Check if the file exists
if (!File.Exists(filePath))
{
    Console.WriteLine($"The file '{filePath}' does not exist.");
    return;
}

// Check if the file is a PNG file
if (Path.GetExtension(filePath).ToLower() != ".png")
{
    Console.WriteLine("The file is not a PNG file. Do you still want to attempt conversion? (Y):");
    int res = Console.Read();
    if (Char.ToLower(Convert.ToChar(res)) != 'y') return;
}

// Determine the output file path
string outputFilePath = args.Length > 1 ? args[1] : Path.ChangeExtension(filePath, ".ico");

// Check if the output path is a directory
if (Directory.Exists(outputFilePath))
{
    string fileName = Path.GetFileNameWithoutExtension(filePath) + ".ico";
    outputFilePath = Path.Combine(outputFilePath, fileName);
}

try
{
    using FileStream stream = File.OpenRead(filePath);
    Image image = Image.FromStream(stream);

    using FileStream outputStream = File.OpenWrite(outputFilePath);
    using var writer = new BinaryWriter(outputStream);

    // Header

    // Reserved (2 bytes): Always set to zero.
    writer.Write((short)0);

    // Type (2 bytes): Specifies the format of the file; for an ICO file, this is 1.
    writer.Write((short)1);

    // Count (2 bytes): Number of images in the file.
    writer.Write((short)1);

    // Directory entries

    // Width (1 byte): Width of the image in pixels; 0 means 256 pixels.
    writer.Write((byte)image.Width);

    // Height (1 byte): Height of the image in pixels; 0 means 256 pixels.
    writer.Write((byte)image.Height);

    // Color Palette (1 byte): Number of colors in the color palette; 0 if the image does not use a palette.
    writer.Write((byte)0);

    // Reserved (1 byte): Should be 0.
    writer.Write((byte)0);

    // Color Planes (2 bytes): For icons, this should be 0 or 1.
    writer.Write((short)0);

    // Bit Count (2 bytes): Bits per pixel.
    writer.Write((short)32);

    // Size of Image Data (4 bytes): Size of the image data in bytes.
    writer.Write((int)stream.Length);

    // Offset of Image Data (4 bytes): Offset of the image data in the ICO file.
    writer.Write(22); // 22 is the bytes written so far + 4 for int

    // Image data

    // This section contains the actual pixel data of the image.
    stream.Position = 0;
    while (stream.Position < stream.Length) writer.Write((byte)stream.ReadByte());

    // Flush wish whoosh
    writer.Flush();

    Console.WriteLine($"The ICO file has been saved as: {outputFilePath}");
}
catch (Exception e)
{
    Console.WriteLine("An error occurred: " + e.Message);
}