using System.IO;

public class TransferPathsTests
{
    [Fact]
    public void Build_WithFolders_AddsStudentPrefixAndRound()
    {
        var directory = TransferPaths.BuildDestinationDirectory(@"\\srv\Velocidad", "Miguel", "RONDA_1", true);
        Assert.Equal(Path.Combine(@"\\srv\Velocidad", "FAST-Miguel", "RONDA_1"), directory);
    }

    [Fact]
    public void Build_KeepsExistingFastPrefix()
    {
        var directory = TransferPaths.BuildDestinationDirectory(@"\\srv\Velocidad", "FAST-Miguel", "RONDA_2", true);
        Assert.Equal(Path.Combine(@"\\srv\Velocidad", "FAST-Miguel", "RONDA_2"), directory);
    }

    [Fact]
    public void Build_WithoutFolders_ReturnsBaseOnly()
    {
        var directory = TransferPaths.BuildDestinationDirectory(@"\\srv\Velocidad", "Miguel", "SIMULTANEA", false);
        Assert.Equal(@"\\srv\Velocidad", directory);
    }

    [Fact]
    public void Build_EmptyStudent_SkipsStudentFolder()
    {
        var directory = TransferPaths.BuildDestinationDirectory(@"\\srv\Velocidad", "  ", "RONDA_1", true);
        Assert.Equal(Path.Combine(@"\\srv\Velocidad", "RONDA_1"), directory);
    }
}
