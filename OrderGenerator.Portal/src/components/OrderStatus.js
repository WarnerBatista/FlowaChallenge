import React from "react";

function OrderStatus({ status, message }) {
  return (
    <section className="status-section">
      <h2>Order Status</h2>
      <p>{status}</p>
      <p>{message}</p>
    </section>
  );
}

export default OrderStatus;
