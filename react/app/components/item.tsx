"use client";
import { ShopContext } from "@/context/shop-context";
import React, { useContext } from "react";

const Item = (props) => {
    const { id, title, image, price, description, category } = props;
    const { addToCart, cartItems } = useContext(ShopContext);
    const cartItemCount = cartItems[id];

    return (
        <div key={id} className="product">
            <div className="content">
                <p className="title">
                    {id}. {title}
                </p>
                <img src={image} alt={title} className="image" />
                <p className="price">${price}</p>
                <p className="description">{description}</p>
                <p className="category">{category}</p>
            </div>
            <button className="addToCartBtn" onClick={() => addToCart(id)}>
                Add to Cart{" "}
                {cartItemCount > 0 && <span>(Quantity: {cartItemCount})</span>}
            </button>
        </div>
    );
}

export default Item;
