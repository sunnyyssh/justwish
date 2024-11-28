package main

import (
	"log"
	"os"

	"github.com/gin-gonic/gin"
	amqp "github.com/rabbitmq/amqp091-go"
)

type SmtpConfig struct {
	Address      string
	NoreplyEmail string
	FromName     string
	Password     string
}

type Config struct {
	Smtp    SmtpConfig
	AmqpUrl string
}

func FailOnError(err error, what string) {
	if err != nil {
		log.Fatalf("%s: %s", what, err)
	}
}

// TODO: This doesn't work. Fix it.
func main() {
	config := loadConfigs()

	amqpConn := initializeAMQPConnection(config.AmqpUrl)
	defer amqpConn.Close()
	channel := initializeAMQPChannel(amqpConn)
	defer channel.Close()

	msgs := initializeQueueConsumer(channel, "send_email_verification")
	smtpSender := initializeSmtpSender(config.Smtp)
	consumeSendEmailQueue(smtpSender, msgs)

	engine := gin.New()
	engine.SetTrustedProxies([]string{"0.0.0.0"})
	engine.Run(":5002")
}

func initializeAMQPConnection(amqpUrl string) *amqp.Connection {
	conn, err := amqp.Dial(amqpUrl)
	FailOnError(err, "failed to connect to AMQP")
	return conn
}

func initializeAMQPChannel(conn *amqp.Connection) *amqp.Channel {
	channel, err := conn.Channel()
	FailOnError(err, "failed to open AMQP channel")
	return channel
}

func initializeQueueConsumer(channel *amqp.Channel, queueName string) <-chan amqp.Delivery {
	queue, err := channel.QueueDeclare(
		queueName,
		true, false, false, false, nil,
	)
	FailOnError(err, "failed to declare queue")

	msgs, err := channel.Consume(
		queue.Name,
		"", true, false, false, false, nil,
	)
	FailOnError(err, "failed to start queue consumer")
	return msgs
}

func initializeSmtpSender(smtpConfig SmtpConfig) *SmtpSender {
	sender, err := NewSmtpSender(&smtpConfig)
	FailOnError(err, "failed to initialize SMTP sender")
	return sender
}

func loadConfigs() *Config {
	return &Config{
		Smtp: SmtpConfig{
			Address:      mustGetEnv("NOTIF_SMTP_ADDRESS"),
			NoreplyEmail: mustGetEnv("NOTIF_SMTP_NOREPLY_EMAIL"),
			FromName:     mustGetEnv("NOTIF_SMTP_FROM_NAME"),
			// Password:     mustGetEnv("NOTIF_SMTP_PASSWORD"),
		},
		AmqpUrl: mustGetEnv("NOTIF_AMQP_URL"),
	}
}

func mustGetEnv(key string) string {
	val, ok := os.LookupEnv(key)
	if !ok {
		log.Fatalf("environment variable %s is missing", key)
	}
	return val
}
