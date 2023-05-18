import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-select',
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: SelectComponent,
      multi: true,
    },
  ],
})
export class SelectComponent {
  @Input() title: string = ''
  @Input() data: any[] = []
  @Input() defaultOption!: string
  selectedValue!: number

  constructor() { }
  onChange!: (value: string) => void;
  onTouched!: () => void;

  writeValue(obj: any): void {
    this.selectedValue = obj;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  ngOnInit(): void { }

}


