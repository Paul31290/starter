import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Validator that checks username format.
 * Username must:
 * - Be 3-50 characters long
 * - Contain only letters, numbers, and underscores
 */
export function usernameValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    if (!value) {
      return null;
    }

    const isLengthValid = value.length >= 3 && value.length <= 50;
    const hasValidFormat = /^[a-zA-Z0-9_]+$/.test(value);

    if (!isLengthValid) {
      return { usernameLength: { min: 3, max: 50, actual: value.length } };
    }

    if (!hasValidFormat) {
      return { usernameFormat: { message: 'Username can only contain letters, numbers, and underscores' } };
    }

    return null;
  };
}

