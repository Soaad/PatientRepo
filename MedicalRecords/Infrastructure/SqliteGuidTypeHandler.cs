using System.Data;
using Dapper;

namespace MedicalRecords.Infrastructure;

public class SqliteGuidTypeHandler: SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
    }

    public override Guid Parse(object value)
    {
        return Guid.Parse(value.ToString());
    }
}
