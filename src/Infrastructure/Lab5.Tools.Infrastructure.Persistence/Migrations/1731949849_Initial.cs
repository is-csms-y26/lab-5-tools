using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Lab5.Tools.Infrastructure.Persistence.Migrations;

[Migration(1731949849, "initial")]
public class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
    """
    create type order_state as enum
    (
        'created',
        'pending_approval',
        'approved',
        'packing',
        'packed',
        'in_delivery',
        'delivered',
        'cancelled'
    );
    
    create table orders
    (
        order_id bigint primary key not null ,
        order_state order_state not null ,
        order_created_at timestamp with time zone not null ,
        order_updated_at timestamp with time zone not null 
    );
    """;

    protected override string GetDownSql(IServiceProvider serviceProvider) => 
    """
    drop table orders;
    drop type order_state;
    """;
}