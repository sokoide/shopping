package main

import (
	"fmt"
	"net/http"
	"time"

	"github.com/gin-contrib/cors"
	"github.com/gin-gonic/gin"
)

type Options struct {
	port int
}

type Product struct {
	ID          int     `json:"id"`
	Title       string  `json:"title"`
	Price       float32 `json:"price"`
	Description string  `json:"description"`
	Category    string  `json:"category"`
	Image       string  `json:"image"`
}

var o Options = Options{
	port: 8080,
}

var products []Product

const baseUrl = "http://scottmm.local:8080/"

func init() {
	products = []Product{
		Product{1, "Mario", 10.0, "", "Mario's family", baseUrl + "static/Slice 1.png"},
		Product{2, "Luigi", 10.0, "", "Mario's family", baseUrl + "static/Slice 2.png"},
		Product{3, "Peach", 15.0, "", "Mario's family", baseUrl + "static/Slice 3.png"},
		Product{4, "Daisy", 15.0, "", "Mario's family", baseUrl + "static/Slice 4.png"},
		Product{5, "Rosalina", 20.0, "", "Mario's family", baseUrl + "static/Slice 5.png"},
		Product{6, "Yoshi", 10.0, "", "Mario's family", baseUrl + "static/Slice 6.png"},
		Product{7, "Wario", 1.50, "", "Wario's family", baseUrl + "static/Slice 7.png"},
		Product{8, "Waluigi", 1.25, "", "Wario's family", baseUrl + "static/Slice 8.png"},
		Product{9, "Donky Kong", 10.50, "", "Donky's family", baseUrl + "static/Slice 9.png"},
		Product{10, "Diddy Kong", 10.50, "", "Donky's family", baseUrl + "static/Slice 10.png"},
		Product{11, "Hammer Bro", 9.50, "", "Bowser's family", baseUrl + "static/Slice 11.png"},
		Product{12, "Hammer Bro Red", 9.50, "", "Bowser's family", baseUrl + "static/Slice 12.png"},
		Product{13, "Bowser", 9.50, "", "Bowser's family", baseUrl + "static/Slice 13.png"},
		Product{14, "Bowser Jr", 9.50, "", "Bowser's family", baseUrl + "static/Slice 14.png"},
		Product{15, "Boo", 9.50, "", "Bowser's family", baseUrl + "static/Slice 15.png"},
		Product{16, "Goomba", 9.50, "", "Bowser's family", baseUrl + "static/Slice 16.png"},
		Product{17, "Maskass", 9.50, "", "Bowser's family", baseUrl + "static/Slice 17.png"},
		Product{18, "Dry Bones", 9.50, "", "Bowser's family", baseUrl + "static/Slice 18.png"},
	}
}

func getProducts(c *gin.Context) {
	c.IndentedJSON(200, products)
}

func main() {
	router := gin.Default()

	// CORS configuration
	config := cors.Config{
		AllowOrigins:     []string{"http://localhost", "http://localhost:3000", "http://example.com", "http://foo.com"},
		AllowMethods:     []string{"GET", "POST", "PUT", "DELETE"},
		AllowHeaders:     []string{"Origin", "Content-Type", "Accept"},
		ExposeHeaders:    []string{"Content-Length"},
		AllowCredentials: true,
		MaxAge:           12 * time.Hour,
	}

	// Enable CORS middleware
	router.Use(cors.New(config))

	router.StaticFS("/static", http.Dir("./static"))

	router.GET("/products", getProducts)

	router.Run(fmt.Sprintf(":%d", o.port))
}
