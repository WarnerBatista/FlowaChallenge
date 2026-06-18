import React, { useState } from "react";

function OrderForm({ onSubmit }) {
  const [symbol, setSymbol] = useState("");
  const [side, setSide] = useState("");
  const [quantity, setQuantity] = useState(0);
  const [price, setPrice] = useState(0);

  const [errors, setErrors] = useState({});

  const validate = () => {
    const newErrors = {};
    if (!symbol) newErrors.symbol = "Symbol is required.";
    if (!side) newErrors.side = "Side is required.";
    if (!quantity || quantity <= 0 || quantity >= 100000) newErrors.quantity = "Quantity must be greater than 0 and less than 100,000.";
    if (!price || price <= 0 || price >= 1000) newErrors.price = "Price must be greater than 0 and less than 1,000.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!validate()) return;
    onSubmit({ symbol, side, quantity, price });
  };

  return (
    <form onSubmit={handleSubmit} className="form-section">
      <h2>Send Order</h2>

      <div className="form-group">
        <label>Symbol</label>
        <select value={symbol} onChange={(e) => setSymbol(e.target.value)}>
          <option value="">Select...</option>
          <option value="PETR4">PETR4</option>
          <option value="VALE3">VALE3</option>
          <option value="VIIA4">VIIA4</option>
        </select>
        {errors.symbol && <span className="error">{errors.symbol}</span>}
      </div>

      <div className="form-group">
        <label>Side</label>
        <select value={side} onChange={(e) => setSide(e.target.value)}>
          <option value="">Select...</option>
          <option value="Buy">Buy</option>
          <option value="Sell">Sell</option>
        </select>
        {errors.side && <span className="error">{errors.side}</span>}
      </div>

      <div className="form-group">
        <label>Quantity</label>
        <input
          type="number"
          value={quantity}
          onChange={(e) => setQuantity(e.target.value)}
        />
        {errors.quantity && <span className="error">{errors.quantity}</span>}
      </div>

      <div className="form-group">
        <label>Price</label>
        <input
          type="number"
          value={price}
          onChange={(e) => setPrice(e.target.value)}
        />
        {errors.price && <span className="error">{errors.price}</span>}
      </div>

      <button className="submit-btn" type="submit">Send Order</button>
    </form>
  );
}

export default OrderForm;
