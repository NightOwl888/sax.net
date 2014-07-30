namespace Sax.Tests.Helpers {
  using Sax.Net.Helpers;

  using Xunit;

  public class AttributesTests {
    public class When_creating_a_new_instance_using_copy_constructor {
      [Fact]
      public void Length_should_equal_original_length() {
        var attributes = new Attributes();
        attributes.AddAttribute(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        attributes.AddAttribute(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

        var copy = new Attributes(attributes);

        Assert.Equal(copy.Length, attributes.Length);
      }
    }
  }
}
