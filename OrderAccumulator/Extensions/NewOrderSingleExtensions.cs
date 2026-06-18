using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderAccumulator.Extensions;

public static class NewOrderSingleExtensions
{
    public static ExecutionReport GenerateNewExecutionReport(this NewOrderSingle order)
    {
        return new ExecutionReport(
            new OrderID(order.ClOrdID.Value),
            new ExecID(Guid.NewGuid().ToString()),
            new ExecType(ExecType.NEW),
            new OrdStatus(OrdStatus.NEW),
            order.Symbol,
            order.Side,
            new LeavesQty(0),
            new CumQty(order.OrderQty.Value),
            new AvgPx(order.Price.Value)
        );
    }

    public static ExecutionReport GenerateRejectedExecutionReport(this NewOrderSingle order)
    {
        return new ExecutionReport(
            new OrderID(order.ClOrdID.Value),
            new ExecID(Guid.NewGuid().ToString()),
            new ExecType(ExecType.REJECTED),
            new OrdStatus(OrdStatus.REJECTED),
            order.Symbol,
            order.Side,
            new LeavesQty(0),
            new CumQty(order.OrderQty.Value),
            new AvgPx(order.Price.Value)
        );
    }
}
