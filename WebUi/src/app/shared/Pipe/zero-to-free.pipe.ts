import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'zeroToFree'
})
export class ZeroToFreePipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    if (value == 0 || value == '$0.00') {
      return 'Free';
    }
    return value;
  }
}
