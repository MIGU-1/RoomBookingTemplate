using MahApps.Metro.Controls;
using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using RoomBooking.Core.Validations;
using RoomBooking.Persistence;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Input;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RoomBooking.Wpf.ViewModels
{
    public class EditCustomerViewModel : BaseViewModel
    {
        private Customer _customer;
        private Customer _undoCustomer;
        private string _firstName;
        private string _lastName;
        private string _iban;
        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
                Validate();
            }
        }
        
        [MinLength(2, ErrorMessage = "Der Vorname muss mindestens 2. Zeichen lang sein!")]
        [MaxLength(50, ErrorMessage = "Der Vorname darf max 50. Zeichen lang sein!")]
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
                Validate();
            }
        }
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
                Validate();
            }
        }
        public string Iban
        {
            get => _iban;
            set
            {
                _iban = value;
                OnPropertyChanged(nameof(Iban));
                Validate();
            }
        }
        public EditCustomerViewModel(IWindowController windowController, Customer customer) : base(windowController)
        {
            Customer = customer;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Iban = customer.Iban;

            _undoCustomer = new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Iban = customer.Iban
            };
        }

        private ICommand _cmdUndoCommand;
        public ICommand CmdUndoCommand
        {
            get
            {
                if (_cmdUndoCommand == null)
                {
                    _cmdUndoCommand = new RelayCommand(
                        execute: _ =>
                        {
                            FirstName = _undoCustomer.FirstName;
                            LastName = _undoCustomer.LastName;
                            Iban = _undoCustomer.Iban;

                        },
                        canExecute: _ => Customer != _undoCustomer);
                }

                return _cmdUndoCommand;
            }

            set => _cmdUndoCommand = value;
        }

        private ICommand _cmdSaveCommand;
        public ICommand CmdSaveCommand
        {
            get
            {
                if (_cmdSaveCommand == null)
                {
                    _cmdSaveCommand = new RelayCommand(
                        execute: async _ =>
                        {
                            using IUnitOfWork uow = new UnitOfWork();
                            Customer.FirstName = FirstName;
                            Customer.LastName = LastName;
                            Customer.Iban = Iban;
                            uow.Customers.Update(Customer);
                            await uow.SaveAsync();
                            Controller.CloseWindow(this);
                        },
                        canExecute: _ => Customer != null);
                }

                return _cmdSaveCommand;
            }

            set => _cmdSaveCommand = value;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(!IbanChecker.CheckIban(Iban))
            {
                yield return new ValidationResult($"Iban nicht gültig!", new string[] { nameof(Iban) });
            }
        }
    }
}
