import React, { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import axios from "axios";
import "./App.css";

function App() {
  const [symbol, setSymbol] = useState("");
  const [side, setSide] = useState("");
  const [quantity, setQuantity] = useState(0);
  const [price, setPrice] = useState(0);
  const [orderId, setOrderId] = useState(null);
  const [status, setStatus] = useState("");
  const [message, setMessage] = useState("No orders have been sent yet.");

  useEffect(() => {
    if (orderId) {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5228/orderHub")
        .withAutomaticReconnect()
        .build();

      connection.start().then(() => {
        connection.invoke("JoinOrderGroup", orderId);
      });

      connection.on("OrderStatusUpdated", (update) => {
        setStatus(`Order ${update.orderId} → Status: ${update.status}`);
        setMessage(update.message);
      });

      return () => {
        connection.stop();
      };
    }
  }, [orderId]);

  const submitOrder = async () => {
    try {
      const response = await axios.post("http://localhost:5228/api/orders/send", {
        symbol,
        side,
        quantity,
        price,
      });
      setOrderId(response.data.orderId);
      setStatus(response.data.status);
      setMessage(response.data.message);
    } catch (err) {
      setMessage("Error sending order.");
    }
  };

  return (
    <div className="app-container">
      <header>
        <h1>Order Generator Portal</h1>
      </header>

      <main>
        <section className="form-section">
          <h2>Send Order</h2>
          <div className="form-group">
            <label>Symbol</label>
            <select value={symbol} onChange={(e) => setSymbol(e.target.value)}>
              <option value="">Select...</option>
              <option value="PETR4">PETR4</option>
              <option value="VALE3">VALE3</option>
              <option value="VIIA4">VIIA4</option>
            </select>
          </div>

          <div className="form-group">
            <label>Side</label>
            <select value={side} onChange={(e) => setSide(e.target.value)}>
              <option value="">Select...</option>
              <option value="Buy">Buy</option>
              <option value="Sell">Sell</option>
            </select>
          </div>

          <div className="form-group">
            <label>Quantity</label>
            <input
              type="number"
              value={quantity}
              onChange={(e) => setQuantity(e.target.value)}
            />
          </div>

          <div className="form-group">
            <label>Price</label>
            <input
              type="number"
              value={price}
              onChange={(e) => setPrice(e.target.value)}
            />
          </div>

          <button className="submit-btn" onClick={submitOrder}>
            Send Order
          </button>
        </section>

        <section className="status-section">
          <h2>Order Status</h2>
          <p>{status}</p>
          <p>{message}</p>
        </section>
      </main>
    </div>
  );
}

export default App;
