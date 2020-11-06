using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
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
        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
            }
        }
        public EditCustomerViewModel(IWindowController windowController, Customer customer) : base(windowController)
        {
            Customer = customer;
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
                            Customer.FirstName = _undoCustomer.FirstName;
                            Customer.LastName = _undoCustomer.LastName;
                            Customer.Iban = _undoCustomer.Iban;

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
            throw new NotImplementedException();
        }
    }
}
