import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RoleCreateRequest } from 'src/app/interfaces/role-create-request';

@Component({
  selector: 'app-role-form',
  templateUrl: './role-form.component.html',
  styleUrls: ['./role-form.component.css']
})
export class RoleFormComponent {
  @Input({ required:true }) role!:RoleCreateRequest
  @Input() errorMessage!:string 

  @Output() addRole:EventEmitter<RoleCreateRequest> = new EventEmitter<RoleCreateRequest>

  add(){
    this.addRole.emit(this.role);
  }
}
