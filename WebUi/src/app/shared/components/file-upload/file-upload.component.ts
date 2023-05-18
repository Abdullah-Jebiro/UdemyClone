import { ChangeDetectionStrategy, Component, EventEmitter, forwardRef, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { SubSink } from 'subsink';
import * as cuid from 'cuid';
import { FileType } from '../../models/FileType';
import { FileUploadService } from 'src/app/services/file-upload.service';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FileUploadComponent),
      multi: true
    }
  ],
})
export class FileUploadComponent implements OnInit, ControlValueAccessor, OnDestroy {

  @Input() fileType!: FileType; // If no value is specified for fileType an exception is raised
  @Input() title = '';
  @Input() directory!: string | number;


  fileName = '';
  uploadProgress = 0;

  private subs = new SubSink();

  constructor(private fileUploadService: FileUploadService, private toastrService: ToastrService) { }

  onChange!: (value: string) => void;
  onTouched!: () => void;

  // This method is called when a value is set programmatically
  writeValue(obj: any): void {
    this.fileName = obj;
  }

  // This method is called when the value in the view changes
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  // This method is called when the control is "touched"
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  ngOnInit(): void { }

  onFileSelected(event: any): void {

    if (!this.directory) {
      throw new Error('Directory is required');
    }
    // Get the selected file
    const file: File = event.target.files[0];
    // Generate a new file name using the cuid library
    const extension = file.name.split('.').pop()!;
    // Check if file type are specified
    if (!this.fileType) {
      throw new Error('File type is required');
    }
    // Check the file type against the expected file type
    else if (this.fileType === 'Image' && !['jpg', 'jpeg', 'png', 'gif', 'webp'].includes(extension.toLowerCase())) {
      this.toastrService.error('Invalid file type.', 'Please upload an image file.', {
        timeOut: 3000,
      });
      throw new Error('Invalid file type. Please upload a image file.');
    } else if (this.fileType === 'Video' && !['mp4', 'avi', 'mov'].includes(extension.toLowerCase())) {
      this.toastrService.error('Invalid file type.', 'Please upload a video file.', {
        timeOut: 3000,
      });
      throw new Error('Invalid file type. Please upload a video file.');
    }
    const newFileName = cuid() + '.' + extension;
    // Call the onChange function with the new file name
    this.onChange(this.directory + '/' + newFileName);
    // Upload the file using the FileUploadService
    this.subs.sink = this.fileUploadService.uploadFile(file, newFileName, this.fileType, this.directory.toString()).subscribe({
      next: (event: any) => {
        if (event.status === 'progress') {
          this.uploadProgress = event.message;

        } else if (event.status === 'success') {
          this.fileName = newFileName;
          this.uploadProgress = 0;
        } else {
          this.uploadProgress = 0;
        }
      },
      error: (err) => {
        this.toastrService.error('', err, {
          timeOut: 5000
        });
      }
    });
  }

  // This method is called when the component is destroyed
  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

}
