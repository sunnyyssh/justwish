package main

import (
	"encoding/json"
	"fmt"
	"log"

	amqp "github.com/rabbitmq/amqp091-go"
)

const verifCodeMessage = "Don't tell enybody your verification code: %d\nEnter it to verify your email."

func consumeSendEmailQueue(sender *SmtpSender, msgs <-chan amqp.Delivery) {
	// 5 is a worker pool size for now.
	for i := 0; i < 5; i++ {
		go func() {
			for m := range msgs {
				handleSendEmailMess(sender, m)
			}
		}()
	}
}

type sendEmailRequest struct {
	Email string `json:"email"`
	Code  int32  `json:"code"`
}

func handleSendEmailMess(sender *SmtpSender, m amqp.Delivery) {
	req := sendEmailRequest{}
	if err := json.Unmarshal(m.Body, &req); err != nil {
		log.Println("error while deserializing amqp message body: ", err)
		return
	}
	err := sender.Send(req.Email, fmt.Sprintf(verifCodeMessage, req.Code))
	if err != nil {
		log.Println("error while sending email: ", err)
		return
	}
}
