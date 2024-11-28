package main

import "net/smtp"

type SmtpSender struct {
	address string
	from    string
	auth    smtp.Auth
}

func NewSmtpSender(config *SmtpConfig) (*SmtpSender, error) {
	return &SmtpSender{
		from:    config.NoreplyEmail,
		address: config.Address,
		auth:    nil, // TODO: for production this shouldn't be nil.
	}, nil
}

func (s *SmtpSender) Send(to, message string) error {
	return smtp.SendMail(
		s.address,
		s.auth,
		s.from,
		[]string{to},
		[]byte(message))
}
