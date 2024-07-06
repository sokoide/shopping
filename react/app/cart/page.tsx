"use client";

import React, { useContext } from "react";
import "./cart.css";
import CartItem from "./cartItem";
import { ShopContext } from "@/context/shop-context";

const Cart = () => {
    const { items, cartItems, getTotalCartAmount, emptyCart, checkout } =
        useContext(ShopContext);
    const totalAmount = Math.round(getTotalCartAmount() * 100) / 100;

    return (
        <div className="cart">
            <div>
                <h1>Items in Cart</h1>
            </div>
            <div className="cart">
                {items.map((item) => {
                    if (cartItems[item.id] !== 0) {
                        return <CartItem data={item} key={item.id} />;
                    }
                })}
            </div>

            {totalAmount > 0 ? (
                <div className="checkout">
                    <p className="total">Total: ${totalAmount}</p>
                    <button
                        onClick={() => {
                            checkout();
                        }}
                    >
                        Checkout
                    </button>
                    <button
                        onClick={() => {
                            emptyCart();
                        }}
                    >
                        Empty Cart
                    </button>
                </div>
            ) : (
                <h1> cart is empty</h1>
            )}
        </div>
    );
};

export default Cart;
