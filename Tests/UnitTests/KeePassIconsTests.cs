using KeePassLib;
using Xunit;

namespace PassXYZ.Server.Tests.UnitTests;

public class KeePassIconsTests
{
    [Fact]
    public void DumpPwIconEnumValues()
    {
        var values = Enum.GetValues(typeof(PwIcon))
            .Cast<PwIcon>()
            .Select(x => $"{(int)x} = {x}")
            .OrderBy(x => x);

        foreach (var value in values)
        {
            Console.WriteLine(value);
        }
    }
}
