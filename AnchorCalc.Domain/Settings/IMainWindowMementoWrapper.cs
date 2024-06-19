using System.Runtime.Serialization;

namespace AnchorCalc.Domain.Settings;

public interface IMainWindowMementoWrapper
{
    [DataMember(Name = "left")]
    double Left { get; set; }
    [DataMember(Name = "top")]
    double Top { get; set; }
    [DataMember(Name = "width")]
    double Width { get; set; }
    [DataMember(Name = "height")]
    double Height { get; set; }
    [DataMember(Name = "isMaximized")]
    bool IsMaximized { get; set; }
}