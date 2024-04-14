using MapleAPI.DataType;

namespace MapleBuilder.Control.Data.Item.Interface;

public interface IUpgradeSupport
{
    public MapleStatContainer GetUpgradeStatus();
}