import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormBuilder } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { FileUploadService } from 'src/app/services/file-upload.service';
import { InstructorService } from 'src/app/services/instructor.service';
import { SubSink } from 'subsink';


@Component({
  selector: 'app-instructor-update',
  templateUrl: './instructor-update.component.html',
  styleUrls: ['./instructor-update.component.css']
})
export class InstructorUpdateComponent implements OnInit {
  uploadProgress: number | undefined;
  isLoading: boolean = false;
  oldProfilePictureUrl: string = ''
  private subs = new SubSink();

  constructor(private fileUploadService: FileUploadService,
    private toastrService: ToastrService,
    private fb: FormBuilder,
    private instructorService: InstructorService) {
  }

  userUpdateForm: FormGroup = this.fb.group({
    about: new FormControl(''),
    ProfilePictureUrl: new FormControl(''),
  }
  );

  ngOnInit(): void {
    this.subs.sink = this.instructorService.getInfo().subscribe({
      next: (data) => {
        this.userUpdateForm.patchValue({
          about: data.about,
          ProfilePictureUrl: data.ProfilePictureUrl
        })
        this.oldProfilePictureUrl = data.ProfilePictureUrl
      }, error: (err) => {
        console.log(err); //TODO
      },
    })

  }




  onSubmit() {
    this.isLoading = true;
    this.subs.sink = this.instructorService.updateInfo(this.userUpdateForm.value).subscribe({
      next: (result) => {
        this.toastrService.success(result.message, '', {
          timeOut: 6000,
        });
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        console.log(err);
        this.toastrService.error('', err, {
          timeOut: 6000,
        });
      },
    });
  }
}
