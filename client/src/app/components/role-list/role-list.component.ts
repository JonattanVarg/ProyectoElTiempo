import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Role } from 'src/app/interfaces/role';

@Component({
  selector: 'app-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.css']
})
export class RoleListComponent {
  @Input({required:true}) roles!: Role[] | null 
  @Output() deleteRole:EventEmitter<string> = new EventEmitter<string>()

  delete(id:string){
    this.deleteRole.emit(id)
  }

  trackById(index: number, role: Role): string {
    return role.id;
  }

}
