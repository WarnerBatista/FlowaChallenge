import React, { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import axios from "axios";
import "./App.css";
import OrderForm from "./components/OrderForm";
import OrderStatus from "./components/OrderStatus";

function App() {
  const [orderId, setOrderId] = useState(null);
  const [status, setStatus] = useState("");
  const [message, setMessage] = useState("No orders have been sent yet.");
  const [formErrors, setFormErrors] = useState({});

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
        setStatus(`Order: ${update.orderId} → Status: ${update.status}`);
        setMessage(update.message);
      });

      return () => {
        connection.stop();
      };
    }
  }, [orderId]);

  const submitOrder = async (order) => {
    try {
      const response = await axios.post("http://localhost:5228/api/orders/send", order);
      setOrderId(response.data.orderId);
      setStatus(`Order: ${response.data.orderId} → Status: ${response.data.status}`);
      setMessage(response.data.message);
      setFormErrors({});
    } catch (err) {
      if (err.response && err.response.data && err.response.data.errors) {
        const backendErrors = err.response.data.errors;
        setFormErrors(backendErrors);
      } else {
        setMessage("Error sending order.");
      }
    }
  };

  return (
    <div className="app-container">
      <header>
        <h1>Order Generator Portal</h1>
      </header>
      <main>
        <OrderForm onSubmit={submitOrder} backendErrors={formErrors} />
        <OrderStatus status={status} message={message} />
      </main>
    </div>
  );
}

export default App;
