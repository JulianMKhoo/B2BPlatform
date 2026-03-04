package database

import (
	"context"
	"fmt"
	"log"

	"github.com/redis/go-redis/v9"
)

func ConnectRedis(addr string) *redis.Client {
	client := redis.NewClient(&redis.Options{
		Addr: addr,
	})
	if err := client.Ping(context.Background()).Err(); err != nil {
		log.Fatalf("failed to connect to redis: %v", err)
	}
	fmt.Println("connected to redis")
	return client
}
