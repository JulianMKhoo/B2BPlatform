package config

import "os"

type Config struct {
	AppEnv   string
	Port     string
	RedisURL string
	DbDSN   string
}

func Load() *Config {
	return &Config{
		AppEnv:   getEnv("APP_ENV", "development"),
		Port:     getEnv("PORT", "8081"),
		RedisURL: getEnv("REDIS_URL", "localhost:6379"),
		DbDSN:   getEnv("DB_DSN", "postgres://admin:password123@localhost:5432/b2b_chat?sslmode=disable"),
	}
}

func getEnv(key, fallback string) string {
	if v := os.Getenv(key); v != "" {
		return v
	}
	return fallback
}
