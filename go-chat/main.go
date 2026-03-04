package main

import (
	"context"
	"fmt"

	"go-chat/config"
	"go-chat/database"
	"go-chat/handlers"
	"go-chat/subscriber"

	"github.com/gin-contrib/cors"
	"github.com/gin-gonic/gin"
)

func main() {
	cfg := config.Load()

	db := database.Connect(cfg.DbDSN)
	defer db.Close()

	database.Migrate(db)

	rdb := database.ConnectRedis(cfg.RedisURL)
	defer rdb.Close()

	// Start Redis subscriber in background
	sub := subscriber.New(rdb, db)
	go sub.Listen(context.Background())

	h := handlers.New(db)
	router := gin.Default()
	router.Use(cors.Default())

	router.GET("/health", h.HealthCheck)

	chat := router.Group("/api/chat/v1")
	{
		chat.POST("/workspace/get", h.GetWorkspace)
		chat.POST("/members/get", h.GetMembers)
		chat.POST("/message/send", h.SendMessage)
		chat.POST("/messages/get", h.GetMessages)
	}

	fmt.Printf("chat service starting on :%s\n", cfg.Port)
	router.Run(":" + cfg.Port)
}
